/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using Sirenix.OdinInspector;

[ CreateAssetMenu( fileName = "delayed_event", menuName = "FF/Event/DelayedGameEvent" ) ]
public class DelayedGameEvent : GameEvent
{
	private RecycledTween recycledTween = new RecycledTween();

	public void Raise( float delay )
    {
		Raise();

		recycledTween.Recycle( DOVirtual.DelayedCall( delay, RaiseOthers ), null );
	}

    public void RaiseOthers()
    {
		if( canRaiseOtherEvents && eventsThatWillAlsoBeRaised != null )
			for( var i = 0; i < eventsThatWillAlsoBeRaised.Count; i++ )
				eventsThatWillAlsoBeRaised[ i ].Raise();
	}
}