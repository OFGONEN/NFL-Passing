﻿/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	[ CreateAssetMenu( fileName = "SharedFloatNotifier", menuName = "FF/Data/Shared/Notifier/Float" ) ]
	public class SharedFloatNotifier : SharedDataNotifier< float >
	{
		public void Add( float value )
		{
			SharedValue += value;
		}

		public void Subtract( float value )
		{
			SharedValue -= value;
		}
	}
}