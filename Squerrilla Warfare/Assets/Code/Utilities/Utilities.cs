using System;

public static class Utilities {
	// ReSharper disable once UnusedMember.Global
	public static S GetWithTemporaryValueFor<T, S> (ref T toChange, T temporaryValue, Func<S> func) {
		var old = toChange;
		// ReSharper disable once RedundantAssignment
		toChange = temporaryValue;
		var toReturn = func();
		toChange = old;
		return toReturn;
	}
	// ReSharper disable once UnusedMember.Global
	public static void DoWithTemporaryValueFor<T> (ref T toChange, T temporaryValue, Action action) {
		var old = toChange;
		// ReSharper disable once RedundantAssignment
		toChange = temporaryValue;
		action();
		toChange = old;
	}
}
