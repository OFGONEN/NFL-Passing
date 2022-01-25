using DG.Tweening;

namespace FFStudio
{
	public class RecycledSequence
	{
		private UnityMessage onComplete;
		private Sequence sequence;

		public Sequence Sequence => sequence;

		public void Recycle( Sequence newSequence, UnityMessage onComplete )
		{
			sequence = sequence.KillProper();
			sequence = newSequence;

			this.onComplete = onComplete;

			sequence.OnComplete( OnComplete_Safe );
		}

		public void Recycle( Sequence newSequence )
		{
			sequence = sequence.KillProper();
			sequence = newSequence;

			sequence.OnComplete( OnComplete_Safe );
		}

		public void Kill()
		{
			sequence = sequence.KillProper();
		}

		private void OnComplete_Safe()
		{
			onComplete?.Invoke();
			sequence = null;
		}
	}
}