
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.Audio
{
	public sealed class AudioSourcePool : MonoBehaviour
	{
		public static AudioSource Play( AudioClip clip, AudioMixerGroup mixerGroup, float volume, bool loop)
		{
			return s_Instance?.PlayInstance( clip, mixerGroup, volume, loop);
		}
		public bool PlayOneShot( AudioClip clip, AudioMixerGroup mixerGroup)
		{
			if( s_Instance == false)
			{
				return false;
			}
			s_Instance.PlayOneShotInstance( clip, mixerGroup);
			return true;
		}
		public static bool PlayBackgroundMusic( AudioClip audioClip, AudioMixerGroup mixerGroup, float fadeOutDuration, float fadeInDuration)
		{
			if( s_Instance == false)
			{
				return false;
			}
			s_Instance.PlayBackgroundMusicInstance( audioClip, mixerGroup, fadeOutDuration, fadeInDuration);
			return true;
		}
		AudioSource PlayInstance( AudioClip clip, AudioMixerGroup mixerGroup, float volume, bool loop)
		{
			if( m_LastCheckFrame != Time.frameCount)
			{
				m_LastCheckFrame = Time.frameCount;
				CheckInUse();
			}
			AudioSource audioSource = (m_Pool.Count == 0)?
				Instantiate( m_Prefab, transform, false) : m_Pool.Dequeue();
			
			if( m_NodePool.Count == 0)
			{
				m_Use.AddLast( audioSource);
			}
			else
			{
				var node = m_NodePool.Dequeue();
				node.Value = audioSource;
				m_Use.AddLast( node);
			}
			audioSource.outputAudioMixerGroup = mixerGroup;
			audioSource.volume = volume;
			audioSource.loop = loop;
			audioSource.clip = clip;
			audioSource.Play();
			return audioSource;
		}
		void PlayOneShotInstance( AudioClip clip, AudioMixerGroup mixerGroup)
		{
			if( m_OneShotPool.TryGetValue( mixerGroup, out AudioSource audioSource) == false)
			{
				audioSource = Instantiate( m_Prefab, transform, false);
				m_OneShotPool.Add( mixerGroup, audioSource);
			}
			audioSource.PlayOneShot( clip);
		}
		void PlayBackgroundMusicInstance( AudioClip audioClip, AudioMixerGroup mixerGroup, float fadeOutDuration, float fadeInDuration)
		{
			if( m_BackgroundMutex != false)
			{
				m_BackgroundRequestClip = audioClip;
				m_BackgroundRequest = true;
				return;
			}
			s_Instance.StartCoroutine( PlayBackgroundMusicInternal( audioClip, mixerGroup, fadeOutDuration, fadeInDuration));
		}
		IEnumerator PlayBackgroundMusicInternal( AudioClip audioClip, AudioMixerGroup mixerGroup, float fadeOutDuration, float fadeInDuration)
		{
			if( m_BackgroundCurrentClip != audioClip)
			{
				if( m_BackgroundSource != null)
				{
					m_BackgroundMutex = true;
					
					if( fadeOutDuration > 0.0f)
					{
						float elapsedTime = 0.0f;
						
						while( elapsedTime < fadeOutDuration)
						{
							elapsedTime += Time.unscaledDeltaTime;
							
							if( elapsedTime >= fadeOutDuration)
							{
								break;
							}
							m_BackgroundSource.volume = 1.0f - Mathf.Clamp01( elapsedTime / fadeOutDuration);
							yield return null;
						}
					}
					m_BackgroundSource.volume = 0.0f;
					m_BackgroundSource.Stop();
					m_BackgroundSource = null;
					m_BackgroundCurrentClip = null;
					m_BackgroundMutex = false;
				}
				if( m_BackgroundRequest != false)
				{
					audioClip = m_BackgroundRequestClip;
					m_BackgroundRequestClip = null;
					m_BackgroundRequest = false;
				}
				if( audioClip != null)
				{
					m_BackgroundCurrentClip = audioClip;
					m_BackgroundSource = s_Instance.PlayInstance( 
						audioClip, mixerGroup, 0.0f, true);
					
					if( fadeInDuration > 0.0f)
					{
						float elapsedTime = 0.0f;
						
						while( elapsedTime < fadeInDuration)
						{
							elapsedTime += Time.deltaTime;
							
							if( elapsedTime >= fadeInDuration)
							{
								break;
							}
							m_BackgroundSource.volume = Mathf.Clamp01( elapsedTime / fadeInDuration);
							yield return null;
						}
					}
					m_BackgroundSource.volume = 1.0f;
				}
			}
		}
		void CheckInUse()
		{
			var node = m_Use.First;
			
			while( node != null)
			{
				var current = node;
				node = node.Next;
				
				if( current.Value.isPlaying == false)
				{
					m_Pool.Enqueue( current.Value);
					m_Use.Remove( current);
					m_NodePool.Enqueue( current);
				}
			}
		}
		void Awake()
		{
			if( s_Instance != null)
			{
				Destroy( gameObject);
			}
			else
			{
				s_Instance = this;
				
				if( transform.parent == null)
				{
					DontDestroyOnLoad( gameObject);
				}
			}
		}
		[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		static void InitializeOnLoad()
		{
			var audioSourcePoolobject = new GameObject( "AudioSourcePool");
			AudioSourcePool sourcePool = audioSourcePoolobject.AddComponent<AudioSourcePool>();
			var audioSourceObject = new GameObject( "AudioSource");
			audioSourceObject.transform.SetParent( audioSourcePoolobject.transform);
			sourcePool.m_Prefab = audioSourceObject.AddComponent<AudioSource>();
			sourcePool.m_Prefab.playOnAwake = false;
			audioSourceObject.hideFlags = HideFlags.HideAndDontSave;
		}
		internal static AudioClip BackgroundClip
		{
			get{ return s_Instance?.m_BackgroundCurrentClip; }
		}
		AudioSource m_Prefab;
		int m_LastCheckFrame = -1;
		readonly Queue<AudioSource> m_Pool = new();
		readonly LinkedList<AudioSource> m_Use = new();
		readonly Queue<LinkedListNode<AudioSource>> m_NodePool = new();
		readonly Dictionary<AudioMixerGroup, AudioSource> m_OneShotPool = new();
		AudioClip m_BackgroundRequestClip;
		AudioClip m_BackgroundCurrentClip;
		AudioSource m_BackgroundSource;
		bool m_BackgroundMutex;
		bool m_BackgroundRequest;
		static AudioSourcePool s_Instance;
	}
}
