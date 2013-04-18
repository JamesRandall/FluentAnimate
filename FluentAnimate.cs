using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;

namespace AccidentalFish.UIKit
{
    public interface IFluentAnimateTail
    {
        IFluentAnimateTail Repeat();
        IFluentAnimateTail Autoreverse();
        void Start();
    }

    public class FluentAnimate : IFluentAnimateTail
    {
        private struct Animation
        {
            public double Duration;
            public Action Action;
            public double Delay;
            public UIViewAnimationOptions AnimationOptions;
        }

        private readonly Stack<Animation> _actions = new Stack<Animation>();
        private bool _repeats;
        private bool _reverses;
        private double? _nextDelay;
        private Action _repeaterAction;

        public static FluentAnimate EaseOut(double duration, Action action)
        {
            FluentAnimate animator = new FluentAnimate();
            animator.ThenEaseOut(duration, action);
            return animator;
        }

        public static FluentAnimate EaseIn(double duration, Action action)
        {
            FluentAnimate animator = new FluentAnimate();
            animator.ThenEaseIn(duration, action);
            return animator;
        }

        public static FluentAnimate EaseInOut(double duration, Action action)
        {
            FluentAnimate animator = new FluentAnimate();
            animator.ThenEaseIn(duration, action);
            return animator;
        }

        public static FluentAnimate Linear(double duration, Action action)
        {
            FluentAnimate animator = new FluentAnimate();
            animator.ThenEaseIn(duration, action);
            return animator;
        }

        public FluentAnimate AfterDelay(double delay)
        {
            _nextDelay = delay;
            return this;
        }

        public FluentAnimate ThenEaseIn(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveEaseIn);
            return this;
        }

        public FluentAnimate ThenEaseIn(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveEaseIn);
            return this;
        }

        public FluentAnimate ThenEaseOut(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveEaseOut);
            return this;
        }

        public FluentAnimate ThenEaseOut(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveEaseOut);
            return this;
        }

        public FluentAnimate ThenEaseInOut(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveEaseInOut);
            return this;
        }

        public FluentAnimate ThenEaseInOut(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveEaseInOut);
            return this;
        }

        public FluentAnimate ThenLinear(Action action)
        {
            AddActionWithPreviousDuration(action, UIViewAnimationOptions.CurveLinear);
            return this;
        }

        public FluentAnimate ThenLinear(double duration, Action action)
        {
            AddActionWithDuration(action, duration, UIViewAnimationOptions.CurveLinear);
            return this;
        }

        public IFluentAnimateTail Repeat()
        {
            _repeats = true;
            return this;
        }

        public IFluentAnimateTail Autoreverse()
        {
            _reverses = true;
            return this;
        }

        public void Start()
        {
            Action nextAction = null;
            bool isEmpty = false;
            do
            {
                Animation animation = _actions.Pop();
                isEmpty = !_actions.Any();
                UIViewAnimationOptions options = animation.AnimationOptions;
                if (isEmpty && _reverses) options |= UIViewAnimationOptions.Autoreverse;
                nextAction = CreateChainedAnimationAction(animation, options, nextAction);
                if (isEmpty && _repeats) _repeaterAction = nextAction;
            } while (!isEmpty);
            nextAction();
        }

        #region Helpers

        private Action CreateChainedAnimationAction(Animation animation, UIViewAnimationOptions options, Action nextAction)
        {
            return () =>
                    UIView.Animate(animation.Duration, animation.Delay, options,
                                   () => animation.Action(),
                                   () => { if (nextAction != null) nextAction(); else if (_repeaterAction != null) _repeaterAction(); });
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
            _nextDelay = null;
        }

        #endregion

    }
}