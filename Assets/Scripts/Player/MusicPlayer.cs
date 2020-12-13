using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{

    [System.Serializable]
    public class _song
    {
        public string song_name, song_artist;
        public AudioClip song;
    }

    public AudioMixer AMixer;
    public AudioSource ASource;
    public List<_song> Songs = new List<_song>();

    public float TimeDisplaying = 3.0f;

    private List<_song> unplayed_songs = new List<_song>();
    private List<_song> played_songs = new List<_song>();

    PlayerController PCTRL;
    PlayerUI PUI;

    private void Awake()
    {
        unplayed_songs = Songs;
        PUI = GameObject.FindObjectOfType<PlayerUI>();
        PCTRL = GameObject.FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        StartCoroutine(PlayMus());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            StopMusic(0.25f);
    }

    public void PlaySong(_song song)
    {
        ASource.pitch = 1;
        ASource.volume = 1;

        ASource.PlayOneShot(song.song);
        PUI.DisplaySongFor(song.song_artist, song.song_name, TimeDisplaying);

        unplayed_songs.Remove(song);
        played_songs.Add(song);
    }

    public void StopMusic(float fadeduration)
    {
        LeanTween.value(ASource.pitch, -3, fadeduration / 2).setOnUpdate((float val) =>
          {
              ASource.pitch = val;
          });

        LeanTween.value(ASource.volume, 0, fadeduration).setEaseInOutCubic().setOnUpdate((float val) =>
        {
            ASource.volume = val;
        }).setOnComplete(() =>
        {
            ASource.Stop();
            ASource.clip = null;
        });
    }

    public void AudioDistortion(float pitchdiff, float fadein, float fadeout)
    {
        float destpitch = 1.0f + pitchdiff;
        LeanTween.value(ASource.pitch, destpitch, fadein).setOnUpdate((float v) =>
        {
            ASource.pitch = v;
        }).setOnComplete(() =>
        {
            destpitch = 1.0f;
            LeanTween.value(ASource.pitch, destpitch, fadeout).setOnUpdate((float v) =>
            {
                ASource.pitch = v;
            });
        });
    }

    IEnumerator PlayMus()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        while (true)
        {
            if (!ASource.isPlaying)
            {
                if (!PCTRL.canMove)
                    yield break;

                if (unplayed_songs.Count == 0)
                {
                    unplayed_songs = played_songs;
                    played_songs.Clear();
                }

                _song selected_clip = unplayed_songs[Random.Range(0, unplayed_songs.Count)];
                PlaySong(selected_clip);
            }

            yield return new WaitForSeconds(2.5f);
        }
    }
}
