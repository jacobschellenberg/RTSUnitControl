using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ListHelpers {

	public static bool ContainsAll<T>(this List<T> a, List<T> b)
	{
		return !b.Except(a).Any();
	}
}
