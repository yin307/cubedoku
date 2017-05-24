using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GroupSame : MonoBehaviour
{
	public int sameGroup;
	public int sameNumber;

	public static GroupSame[] getAllGroupSame()
	{
		GroupSame[] res = FindObjectsOfType (typeof(GroupSame)) as GroupSame[];
		return res;
	}

	public List<GroupSame> getMyGroups()
	{
		List<GroupSame> res = new List<GroupSame> ();
		GroupSame[] all = getAllGroupSame ();
		if (all.Length > 0) {
			for (int i = 0; i < all.Length; i++) {
				if (all [i].sameGroup == sameGroup) {
					res.Add (all [i]);
				}
			}
		}
		return res;
	}
}

