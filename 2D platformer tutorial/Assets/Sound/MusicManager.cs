using UnityEngine;
using UnityEngine.UI;
public class MusicManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;


    public void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void OnOffVolume()
    {
        float musictemp = AudioListener.volume;
        float slidertemp = volumeSlider.value;
        if (musictemp != 0 && slidertemp != 0)
        {
            AudioListener.volume = 0;
            volumeSlider.value = 0;
        }
        else
        {
            AudioListener.volume = musictemp;
            volumeSlider.value = slidertemp;
        }

    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();
    }


    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }
}
