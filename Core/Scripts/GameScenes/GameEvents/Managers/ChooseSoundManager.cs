using DI;
using Game_events;
using Godot;
using System;
using System.Linq;

public interface IChooseSoundManager
{

}

public partial class ChooseSoundManager : AudioStreamPlayer2D, IChooseSoundManager, IStartable, IDispose
{
    [Inject] private IEventsManager eventsManager;
    
    [Export] private AudioStreamPlayer2D audioPlayer;
    [Export] private AudioStream goodChoiceSound;
    [Export] private AudioStream badChoiceSound;


    public void Start()
    {
        eventsManager.onChoiceActivate += OnChoice;
    }
    public void Dispose()
    {
        eventsManager.onChoiceActivate -= OnChoice;
    }
    private void OnChoice(ChoiceData data)
    {
        AudioStream stream;

        if(data.actionsData.Any(x => x.changeStatType == ChangeStatType.randDestroy || x.changeStatType == ChangeStatType.subtract))
        {
            stream = badChoiceSound;
        }
        else
        {
            stream = goodChoiceSound;
        }

        if (audioPlayer.Playing)
        {
            audioPlayer.Stop();
        }
        
        audioPlayer.Stream = stream;
        audioPlayer.Play();
    }


}
