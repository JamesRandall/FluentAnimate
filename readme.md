# README

Fluent Animate is a fluent interface for building and chaining UIView animations in C# using MonoTouch.

Really just built it as a quick early morning Xamarin Conference Hack but I will come back and document and test it properly as I think it has promise.

In the meantime its pretty simply to use. Just add the FluentAnimate.cs file to your project and use syntax like the below to simplify chained animations.

    FluentAnimate
      .EaseIn(0.5, () => _button.Center = new PointF(50, 50))
      .Then.EaseInOut(() => _button.Center = new PointF(50, 250))
	  .Then.Do(() => Console.WriteLine("hello"))
      .Then.EaseOut(() => _button.Center = new PointF(250, 250))
      .Then.Linear(() => _button.Center = new PointF(250, 50))
      .Then.After(3.0).Repeat()
      .Start();

The .Then is optional.

.Do allows you to run actions in the chain that are not executed in the context of an animation.

## License

Copyright (C) 2013 James Randall

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.