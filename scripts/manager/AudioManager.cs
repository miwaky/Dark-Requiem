using Raylib_cs;
using static Raylib_cs.Raylib;
using System.Collections.Generic;
using System;

public static class AudioManager
{
    private static Dictionary<string, Sound> sounds = new();
    private static Dictionary<string, Music> musics = new();
    private static Music? currentMusic;
    private static string? currentMusicKey;
    private static bool musicPlaying = false;

    private static float musicVolume = 0.5f;
    private static float sfxVolume = 0.5f;

    public static float MusicVolume => musicVolume;
    public static float SfxVolume => sfxVolume;

    public static void LoadAll()
    {
        InitAudioDevice();
        sounds["attack"] = LoadSound("assets/sound/sfx/Sword.wav");
        sounds["walk"] = LoadSound("assets/sound/sfx/Footsteps_grass_01.wav");
        sounds["slash"] = LoadSound("assets/sound/sfx/SlashObstacleForest.wav");
        sounds["OpenChest"] = LoadSound("assets/sound/sfx/OpenChest.wav");
        sounds["ItemAcquire"] = LoadSound("assets/sound/sfx/ItemAcquire.wav");

        musics["temple"] = LoadMusicStream("assets/sound/music/Temple.ogg");
        musics["forest"] = LoadMusicStream("assets/sound/music/Forest_Loop.ogg");
        musics["menu"] = LoadMusicStream("assets/sound/music/Menu.ogg");
    }

    public static void Play(string key)
    {
        if (sounds.ContainsKey(key))
        {
            var sound = sounds[key];
            SetSoundVolume(sound, sfxVolume);
            PlaySound(sound);
        }
        else
        {
            //Console.WriteLine($"[Audio] Le son \"{key}\" est introuvable !");
        }
    }

    public static void PlayMusic(string key, float volume = 1f)
    {
        if (!musics.ContainsKey(key))
        {
            //Console.WriteLine($"[Audio] Musique \"{key}\" introuvable !");
            return;
        }

        if (musicPlaying && currentMusicKey == key)
            return;

        StopMusic();

        currentMusic = musics[key];
        SetMusicVolume(volume);
        PlayMusicStream(currentMusic.Value);
        currentMusicKey = key;
        musicPlaying = true;
    }

    public static void Update()
    {
        if (musicPlaying && currentMusic.HasValue)
        {
            UpdateMusicStream(currentMusic.Value);
        }
    }

    public static void StopMusic()
    {
        if (musicPlaying && currentMusic.HasValue)
        {
            StopMusicStream(currentMusic.Value);
            currentMusicKey = null;
            musicPlaying = false;
        }
    }

    public static void SetMusicVolume(float volume)
    {
        musicVolume = Math.Clamp(volume, 0f, 1f);
        if (currentMusic.HasValue)
        {
            Raylib.SetMusicVolume(currentMusic.Value, musicVolume);
        }
    }

    public static void SetSfxVolume(float volume)
    {
        sfxVolume = Math.Clamp(volume, 0f, 1f);
    }

    public static void UnloadAll()
    {
        foreach (var sound in sounds.Values)
        {
            UnloadSound(sound);
        }
        sounds.Clear();

        foreach (var music in musics.Values)
        {
            UnloadMusicStream(music);
        }
        musics.Clear();

        musicPlaying = false;
        currentMusicKey = null;
    }
}