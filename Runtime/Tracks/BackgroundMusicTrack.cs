
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.ComponentModel; 

namespace UnityEngine.Audio
{
	[TrackColor( 252.0f / 255.0f, 192.0f / 255.0f, 7.0f / 255.0f)]
	[TrackClipType( typeof( BackgroundMusicClip))]
	[DisplayName( "Knit.Timeline/Background Music Track")]
	sealed class BackgroundMusicTrack : TrackAsset
	{
		public override Playable CreateTrackMixer( PlayableGraph graph, GameObject go, int inputCount)
		{
			var playable = ScriptPlayable<BackgroundMusicMixerBehaviour>.Create( graph, inputCount);
			playable.GetBehaviour().Initialize( m_AudioMixerGroup, m_FadeOutDuration, m_FadeInDuration);
			return playable;
		}
		[SerializeField]
		AudioMixerGroup m_AudioMixerGroup;
		[SerializeField, Min( 0.0f)]
		float m_FadeOutDuration = 0.4f;
		[SerializeField, Min( 0.0f)]
		float m_FadeInDuration = 0.4f;
	}
}
