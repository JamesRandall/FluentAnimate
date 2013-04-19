using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;

namespace AccidentalFish.UIKit
{
    public interface IFluentAnimateTail
    {
        IFluentAnimateTail Repeat();
        IFluentAnimateTail AllowUserInteraction();
        IFluentAnimateTail WhenComplete(Action action);
        void Start();
    }

    public interface IFluentAnimate : IFluentAnimateTail
    {
        IFluentAnimate Then { get; }
        IFluentAnimate Do(Action action);
        IFluentAnimate After(double delay);
        IFluentAnimate AutoReverse();
        IFluentAnimate EaseIn(Action action);
        IFluentAnimate EaseIn(double duration, Action action);
        IFluentAnimate EaseOut(double duration, Action action);
        IFluentAnimate EaseOut(Action action);
        IFluentAnimate Linear(double duration, Action action);
        IFluentAnimate Linear(Action action);
        IFluentAnimate EaseInOut(double duration, Action action);
        IFluentAnimate EaseInOut(Action action);
    }

    public class FluentAnimate : IFluentAnimate
    {
        private struct Animation
        {
            public bool IsActionOnly;
            public double Duration;
            public Action Action;
            public double Delay;
            public UIViewAnimationOptions AnimationOptions;
            public bool AutoReverses;
        }

        private readonly Stack<Animation> _actions = new Stack<Animation>();
        private bool _repeats;
        private double? _nextDelay;
        private double? _repeaterDelay;
        private Action _repeaterAction;
        private bool _allowsUserInteraction;
        private Action _whenCompleteAction;

        public static IFluentAnimate EaseOut(double duration, Action action)
        {
            IFluentAnimate animator = new FluentAnimate();
            animator.EaseOut(duration, action);
            return animator;
        }

        public static IFluentAnimate EaseIn(double duration, Action action)
        {
            IFluentAnimate animator = new FluentAnimate();
            animator.EaseIn(duration, action);
            return animator;
        }

        public static IFluentAnimate EaseInOut(double duration, Action action)
        {
            IFluentAnimate animator = new FluentAnimate();
            animator.EaseIn(duration, action);
            return animator;
        }

        public static IFluentAnimate Linear(double duration, Action action)
        {
            IFluentAnimate animator = new FluentAnimate();
            animator.EaseIn(duration, action);
            return animator;
        }

        #region IFluentAnimate implementation

        IFluentAnimate IFluentAnimate.After(double delay)
        {
            _nextDelay = delay;
            return this;
        }

        IFluentAnimate IFluentAnimate.Then { get { return this; } }

        IFluentAnimate IFluentAnimate.EaseIn(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveEaseIn);
            return this;
        }

        IFluentAnimate IFluentAnimate.EaseIn(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveEaseIn);
            return this;
        }

        IFluentAnimate IFluentAnimate.EaseOut(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveEaseOut);
            return this;
        }

        IFluentAnimate IFluentAnimate.EaseOut(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveEaseOut);
            return this;
        }

        IFluentAnimate IFluentAnimate.EaseInOut(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveEaseInOut);
            return this;
        }

        IFluentAnimate IFluentAnimate.EaseInOut(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveEaseInOut);
            return this;
        }

        IFluentAnimate IFluentAnimate.Linear(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveLinear);
            return this;
        }

        IFluentAnimate IFluentAnimate.Linear(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveLinear);
            return this;
        }

        IFluentAnimate IFluentAnimate.Do(Action action)
        {
            AddDoAction(action);
            return this;
        }

        IFluentAnimate IFluentAnimate.AutoReverse()
        {
            Animation animation = _actions.Pop();
            animation.AutoReverses = true;
            _actions.Push(animation);
            return this;
        }

        #endregion

        #region IFluentAnimateTail implementation

        IFluentAnimateTail IFluentAnimateTail.Repeat()
        {
            _repeaterDelay = _nextDelay;
            _repeats = true;
            return this;
        }

        IFluentAnimateTail IFluentAnimateTail.AllowUserInteraction()
        {
            _allowsUserInteraction = true;
            return this;
        }

        IFluentAnimateTail IFluentAnimateTail.WhenComplete(Action action)
        {
            _whenCompleteAction = action;
            return this;
        }

        void IFluentAnimateTail.Start()
        {
            Action nextAction = null;
            bool stackIsEmpty;
            do
            {
                Animation animation = _actions.Pop();
                stackIsEmpty = !_actions.Any();
                UIViewAnimationOptions options = animation.AnimationOptions;
                if (_allowsUserInteraction) options = options | UIViewAnimationOptions.AllowUserInteraction;
                if (animation.AutoReverses) options |= UIViewAnimationOptions.Autoreverse;
                nextAction = CreateChainedAnimationAction(animation, options, nextAction);
                if (stackIsEmpty && _repeats)
                {
                    if (_repeaterDelay.HasValue)
                    {
                        animation.Delay = _repeaterDelay.Value;
                        _repeaterAction = CreateChainedAnimationAction(animation, options, nextAction);
                    }
                    else
                    {
                        _repeaterAction = nextAction;
                    }
                    _repeaterDelay = null;
                }
                _nextDelay = null;
            } while (!stackIsEmpty);
            nextAction();
        }

        #endregion

        #region Helpers

        private Action CreateChainedAnimationAction(Animation animation, UIViewAnimationOptions options, Action nextAction)
        {
            if (animation.IsActionOnly)
            {
                return () =>
                {
                    animation.Action();
                    if (nextAction != null) nextAction();
                    else
                    {
                        if (_whenCompleteAction != null) _whenCompleteAction();
                        if (_repeaterAction != null) _repeaterAction();
                    }
                };
            }
            return () =>
                    UIView.Animate(animation.Duration, animation.Delay, options,
                                   () => animation.Action(),
                                   () =>
                                   {
                                       if (nextAction != null) nextAction();
                                       else
                                       {
                                           if (_whenCompleteAction != null) _whenCompleteAction();
                                           if (_repeaterAction != null) _repeaterAction();
                                       }
                                   });
        }

        private void AddActionWithPreviousDuration(Action action, UIViewAnimationOptions animationOptions)
        {
            AddActionWithDuration(action, _actions.Last().Duration, animationOptions);
        }

        private void AddActionWithDuration(Action action, double duration, UIViewAnimationOptions animationOptions)
        {
            _actions.Push(new Animation
            {
                Action = action,
                AnimationOptions = animationOptions,
                Duration = duration,
                Delay = _nextDelay.HasValue ? _nextDelay.Value : 0
            });
        }

        private void AddDoAction(Action action)
        {
            _actions.Push(new Animation
            {
                Action = action,
                IsActionOnly = true
            });
        }

        #endregion

    }
}