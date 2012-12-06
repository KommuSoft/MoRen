using System;

namespace Renderer {
	
	public abstract class Lazy<A> {
		
		protected bool eval = false;
		protected A result;
		
		public A Eval () {
			if(!eval) {
				eval = true;
				result = InnerEval();
			}
			return result;
		}
		protected abstract A InnerEval ();
		
	}
	
	public class L<A> : Lazy<A> {
		
		public readonly Func<A> func;
		
		public L (Func<A> func) {
			this.func = func;
		}
		public L (A result) {
			this.eval = true;
			this.result = result;
		}
		
		protected override A InnerEval () {
			return func();
		}
		
	}
	
	public class L<A,B,C> : Lazy<A> {
		
		public readonly B b;
		public readonly C c;
		public readonly Func<B,C,A> func;
		
		public L (Func<B,C,A> func, B b, C c) {
			this.func = func;
			this.b = b;
			this.c = c;
		}
		
		protected override A InnerEval () {
			return func(b,c);
		}
		
	}
	
}