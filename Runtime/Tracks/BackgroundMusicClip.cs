
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace Knit.Timeline
{
	[System.Serializable]
	sealed class BackgroundMusicClip : PlayableAsset, ITimelineClipAsset
	{
		public ClipCaps clipCaps
		{
			get { return ClipCaps.Extrapolation; }
		}
		public override double duration
		{
			get
			{
				if( m_Source.m_AudioClip == null)
				{
					return base.duration;
				}
				return (double)m_Source.m_AudioClip.samples / m_Source.m_AudioClip.frequency;
			}
		}
		public override Playable CreatePlayable( PlayableGraph graph, GameObject owner)
		{
			return ScriptPlayable<BackgroundMusicBehaviour>.Create( graph, m_Source);
		}
		[SerializeField]
		BackgroundMusicBehaviour m_Source = new();
	}
}
