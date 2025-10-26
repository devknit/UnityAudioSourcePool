
using UnityEngine.Playables;

namespace UnityEngine.Audio
{
	[System.Serializable]
	sealed class BackgroundMusicBehaviour : PlayableBehaviour
	{
		[SerializeField]
		internal AudioClip m_AudioClip;
		[SerializeField, Min( 0.0f)]
		internal float m_FadeOutDuration = 0.4f;
		[SerializeField, Min( 0.0f)]
		internal float m_FadeInDuration = 0.4f;
	}
}
