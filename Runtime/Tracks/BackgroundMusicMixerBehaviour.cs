
using UnityEngine.Playables;

namespace UnityEngine.Audio
{
	sealed class BackgroundMusicMixerBehaviour : PlayableBehaviour
	{
		internal void Initialize( AudioMixerGroup mixerGroup, float fadeOutDuration, float fadeInDuration)
		{
			m_AudioMixerGroup = mixerGroup;
			m_FadeOutDuration = fadeOutDuration;
			m_FadeInDuration = fadeInDuration;
		}
		public override void OnBehaviourPlay( Playable playable, FrameData info)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				return;
			}
		#endif
			m_DefaultAudioClip = m_CurrentAudioClip = AudioSourcePool.BackgroundClip;
		}
		public override void ProcessFrame( Playable playable, FrameData info, object playerData)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				return;
			}
		#endif
			int inputCount = playable.GetInputCount();
			AudioClip audioClip = null;
			float fadeOutDuration = m_FadeOutDuration;
			float fadeInDuration = m_FadeInDuration;
			
			for( int i0 = 0; i0 < inputCount; ++i0)
			{
				if( playable.GetInputWeight( i0) > 0.0f)
				{
					var inputPlayable = (ScriptPlayable<BackgroundMusicBehaviour>)playable.GetInput( i0);
					BackgroundMusicBehaviour behaviour = inputPlayable.GetBehaviour();
					audioClip = behaviour.m_AudioClip;
					fadeOutDuration = behaviour.m_FadeOutDuration;
					fadeInDuration = behaviour.m_FadeInDuration;
					break;
				}
			}
			if( m_CurrentAudioClip != audioClip)
			{
				AudioSourcePool.PlayBackgroundMusic( audioClip, m_AudioMixerGroup, fadeOutDuration, fadeInDuration);
				m_CurrentAudioClip = audioClip;
			}
		}
		public override void OnPlayableDestroy( Playable playable)
		{
		#if UNITY_EDITOR
			if( Application.isPlaying == false)
			{
				return;
			}
		#endif
			if( m_DefaultAudioClip != m_CurrentAudioClip)
			{
				AudioSourcePool.PlayBackgroundMusic( m_DefaultAudioClip, m_AudioMixerGroup, m_FadeOutDuration, m_FadeInDuration);
			}
		}
		AudioClip m_CurrentAudioClip;
		AudioClip m_DefaultAudioClip;
		AudioMixerGroup m_AudioMixerGroup;
		float m_FadeOutDuration;
		float m_FadeInDuration;
	}
}
