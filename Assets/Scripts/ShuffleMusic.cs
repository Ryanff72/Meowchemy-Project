using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleMusic : MonoBehaviour
{
    public List<AudioClip> musicList;
    private AudioSource audioSource;
    private int currentTrackIndex = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ShuffleMusicList();
        PlayCurrentTrack();
    }

    void ShuffleMusicList()
    {
        for (int i = 0; i < musicList.Count; i++)
        {
            AudioClip temp = musicList[i];
            int randomIndex = Random.Range(i, musicList.Count);
            musicList[i] = musicList[randomIndex];
            musicList[randomIndex] = temp;
        }
    }

    void PlayCurrentTrack()
    {
        audioSource.clip = musicList[currentTrackIndex];
        audioSource.Play();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            currentTrackIndex++;
            if (currentTrackIndex >= musicList.Count)
            {
                currentTrackIndex = 0;
                ShuffleMusicList();
            }
            PlayCurrentTrack();
        }
    }
}

