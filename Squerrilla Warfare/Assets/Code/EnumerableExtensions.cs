using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static partial class Extensions {
	public static void ForEach<T> (this IEnumerable<T> enumerable, Action<T> action) {
		foreach (var t in enumerable)
			action(t);
	}
	public static IEnumerable<T> Preferably<T> (this IEnumerable<T> enumerable, Func<T, bool> predicate) {return enumerable.Any(predicate) ? enumerable.Where(predicate) : enumerable;}
	public static void IdempotentAdd<T> (this List<T> list, T toAdd) {
		if (!list.Contains(toAdd))
			list.Add(toAdd);
	}
	public static IEnumerable<T> FirstAsSingletonOrEmpty<T> (this IEnumerable<T> enumerable, Func<T, bool> predicate) {return enumerable.Where(predicate).FirstAsSingletonOrEmpty();}
	public static IEnumerable<T> FirstAsSingletonOrEmpty<T> (this IEnumerable<T> enumerable) {
		return enumerable.Any() ? enumerable.First().AsSingleton() : Enumerable.Empty<T>();
	}
	public static IEnumerable<T> AsSingleton<T> (this T t) {yield return t;}
	public static IEnumerable<T> NotOfType<T, Subtype> (this IEnumerable<T> enumerable) where Subtype : T {return enumerable.Where(t => !(t is Subtype));}
	struct NonAllocatingSortEnumerator<T> : IEnumerator<T> {
		IEnumerable<T> source;
		Comparison<T> comparison;
		T current;
		int equivalentPassed;
		int countOfCurrent;
		public NonAllocatingSortEnumerator (IEnumerable<T> source, Comparison<T> comparison) {
			this.source = source;
			this.comparison = comparison;
			current = default(T);//don't care.
			equivalentPassed = -1;
			countOfCurrent = 0;
		}
		bool ComparesSameAsCurrent (T t) {return comparison(current, t) == 0;}
		bool ComparesPastCurrent (T t) {return comparison(current, t) < 0;}
		public bool MoveNext () {
			var movingToFirst = equivalentPassed == -1;
			if (PastEnd) return false;
			if (++equivalentPassed == countOfCurrent) {
				equivalentPassed = 0;
				var beyondEquivalentGroup = movingToFirst ? source : source.Where(ComparesPastCurrent);
				if (!beyondEquivalentGroup.Any()) return false;
				current = beyondEquivalentGroup.MinOrDefault(comparison);
				countOfCurrent = source.Count(ComparesSameAsCurrent);
			}
			current = source.Where(ComparesSameAsCurrent).Skip(equivalentPassed).First();
			return true;
		}
		bool PastEnd {get {return equivalentPassed > countOfCurrent;}}
		public void Reset () {
			current = default(T);//don't care.
			equivalentPassed = -1;
		}
		public T Current {get {if (PastEnd) throw new InvalidOperationException("Past end."); return current;}}
		object IEnumerator.Current {get {return Current;}}
		public void Dispose () {}
	}
	public static IEnumerable<T> Sorted<T> (this IEnumerable<T> enumerable, Comparison<T> comparison) {
		if (!enumerable.Skip(1).Any())
			return enumerable;//Performance.
		var copy = enumerable.ToList();
		copy.Sort(comparison);
		return copy;
	}
	//This is an O(n^2) sort. But it doesn't allocate a copied collection on the heap.
	//Intended for sorts that run hundreds of times per frame on collections that are usually very small.
	public static IEnumerable<T> NonAllocatinglySorted<T> (this IEnumerable<T> enumerable, Comparison<T> comparison) {
		return !enumerable.Skip(1).Any() ? enumerable : enumerable.NonAllocatinglySortedWithoutShortcut(comparison);
	}
	public static IEnumerable<T> NonAllocatinglySortedWithoutShortcut<T> (this IEnumerable<T> enumerable, Comparison<T> comparison) {
		var enumerator = new NonAllocatingSortEnumerator<T>(enumerable, comparison);
		while (enumerator.MoveNext())
			yield return enumerator.Current;
	}
	public static T MinOrDefault<T> (this IEnumerable<T> enumerable, Comparison<T> comparison) {
		var runningMin = default(T);
		var assignedYet = false;
		var enumerator = enumerable.GetEnumerator();
		while (enumerator.MoveNext()) {
			if (!assignedYet || comparison(runningMin, enumerator.Current) > 0) {
				assignedYet = true;
				runningMin = enumerator.Current;
			}
		}
		return runningMin;
	}
	public static T MaxOrDefault<T> (this IEnumerable<T> enumerable, Comparison<T> comparison) {
		var runningMax = default(T);
		var assignedYet = false;
		var enumerator = enumerable.GetEnumerator();
		while (enumerator.MoveNext()) {
			if (!assignedYet || comparison(runningMax, enumerator.Current) < 0) {
				assignedYet = true;
				runningMax = enumerator.Current;
			}
		}
		return runningMax;
	}
	public static T AggregateOrDefault<T> (this IEnumerable<T> enumerable, Func<T, T, T> accumulator) {
		return enumerable.Any() ? enumerable.Aggregate(accumulator) : default(T);
	}
	public static List<T> With <T> (this IEnumerable<T> enumerable, T extra) {
		var list = enumerable.ToList();
		list.Add(extra);
		return list;
	}
	public static List<T> WithFirst<T> (this IEnumerable<T> enumerable, T extra) {
		var list = new List<T> {extra};
		list.AddRange(enumerable);
		return list;
	}
	public static List<T> WithRange <T> (this IEnumerable<T> enumerable, IEnumerable<T> other) {
		var list = enumerable.ToList();
		list.AddRange(other);
		return list;
	}
	public static T Only <T> (this IEnumerable<T> enumerable) {
		#if UNITY_EDITOR
			if (enumerable.Count() != 1)
				throw new Exception("Enumerable should have been a singleton, but had " + enumerable.Count() + " elements.");
		#endif
		return enumerable.FirstOrDefault();
	}
	public static T OnlyOrDefault <T> (this IEnumerable<T> enumerable) {
		#if UNITY_EDITOR
			if (enumerable.Count() > 1)
				throw new Exception("Enumerable should have had 1 or 0 elements, but had " + enumerable.Count() + ".");
		#endif
		return enumerable.FirstOrDefault();
	}
	public static T FirstOrThrow <T> (this IEnumerable<T> enumerable, Exception exception) {
		if (enumerable.Any())
			return enumerable.First();
		throw exception;
	}
	public static T ArgMax<T> (this IEnumerable<T> enumerable, Func<T, float> scorer) {
		if (!enumerable.Any())
			throw new Exception("ArgMax is undefined for an empty collection.");
		var argMax = enumerable.First();
		var max = scorer(argMax);
		foreach (var element in enumerable) {
			var score = scorer(element);
			if (score > max) {
				max = score;
				argMax = element;
			}
		}
		return argMax;
	}
	public static T ArgMin<T> (this IEnumerable<T> enumerable, Func<T, float> scorer) {
		if (!enumerable.Any())
			throw new Exception("ArgMin is undefined for an empty collection.");
		var argMin = enumerable.First();
		var min = scorer(argMin);
		foreach (var element in enumerable) {
			var score = scorer(element);
			if (score < min) {
				min = score;
				argMin = element;
			}
		}
		return argMin;
	}
	public static IEnumerable<T> ArgMaxInEnumerable<T> (this IEnumerable<T> original, Func<T, float> scorer) {
		return original.Any() ? original.ArgMax(scorer).AsSingleton() : Enumerable.Empty<T>();
	}
	public static IEnumerable<T> ArgMinInEnumerable<T> (this IEnumerable<T> original, Func<T, float> scorer) {
		return original.Any() ? original.ArgMin(scorer).AsSingleton() : Enumerable.Empty<T>();
	}
	public static T RandomElement<T> (this IEnumerable<T> enumerable) {
		return enumerable == null ? default(T) : enumerable.ToArray().RandomElement();
	}
	public static int RandomIndex<T> (this IEnumerable<T> enumerable) {
		return enumerable == null ? 0 : Random.Range(0, enumerable.Count());
	}
	public static int DuplicateCount<T> (this IEnumerable<T> enumerable) {
		var set = new HashSet<T>();
		var count = 0;
		var enumerator = enumerable.GetEnumerator();
		while (enumerator.MoveNext())
			if (set.Contains(enumerator.Current))
				++count;
			else
				set.Add(enumerator.Current);
		return count;
	}
	public static IEnumerable<T> Except<T> (this IEnumerable<T> enumerable, T exception) {
		return enumerable.Where(t => !t.Equals(exception));
	}
	public static IEnumerable<T> Except<T> (this IEnumerable<T> enumerable, params T[] exceptions) {
		return enumerable.ExceptRange(exceptions);
	}
	public static IEnumerable<T> ExceptRange<T, S> (this IEnumerable<T> enumerable, IEnumerable<S> exceptions) where S : T {
		return enumerable.Where(t => !exceptions.Any(exception => t.Equals(exception)));
	}
	public static Vector3 Average<T> (this IEnumerable<T> enumerable, Func<T, Vector3> selector) {
		var sum = Vector3.zero;
		var count = 0;
		foreach (var element in enumerable) {
			sum += selector(element);
			++count;
		}
		return sum / count;
	}
	public static T After<T> (this IEnumerable<T> enumerable, T startPoint) {//wraps.
		var list = enumerable.ToList();
		var currentIndex = list.IndexOf(startPoint);
		var newIndex = (currentIndex + 1) % list.Count;
		return list[newIndex];
	}
	public static T Before<T> (this IEnumerable<T> enumerable, T startPoint) {//wraps.
		var list = enumerable.ToList();
		var currentIndex = list.IndexOf(startPoint);
		var newIndex = (currentIndex - 1 + list.Count) % list.Count;
		return list[newIndex];
	}
	public static bool ValidIndex<T> (this IEnumerable<T> enumerable, int index) {
		return index >= 0 && enumerable.ValidIndex((uint) index);
	}
	public static bool ValidIndex<T> (this IEnumerable<T> enumerable, uint index) {
		return index < enumerable.LongCount();
	}
	public static IEnumerable<T> Distinct<T, S> (this IEnumerable<T> enumerable, Func<T, S> selector) {
		var hashSet = new HashSet<S>();
		return enumerable.Where(t => hashSet.Add(selector(t)));
	}
	public static bool SetEquals<T> (this IEnumerable<T> enumerable, IEnumerable<T> other) {return enumerable.ToHashSet().SetEquals(other.ToHashSet());}
	public static Dictionary<S, List<T>> GroupedBy<T, S> (this IEnumerable<T> enumerable, Func<T, S> selector) {
		var toReturn = new Dictionary<S, List<T>>();
		foreach (var t in enumerable) {
			var s = selector(t);
			if (toReturn.ContainsKey(s))
				toReturn[s].Add(t);
			else
				toReturn[s] = new List<T> {t};
		}
		return toReturn;
	}
	public static HashSet<T> ToHashSet<T> (this IEnumerable<T> enumerable) {
		var hashset = new HashSet<T>();
		foreach (var t in enumerable)
			hashset.Add(t);
		return hashset;
	}
}
