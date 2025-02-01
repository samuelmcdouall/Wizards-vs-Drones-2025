public class WVDEventDataDisplayTutorial : WVDEventData
{
    public WVDTutorialManager.TutorialPart Part;
    public float Delay;

    public WVDEventDataDisplayTutorial(WVDTutorialManager.TutorialPart part, float delay)
    {
        Part = part;
        Delay = delay;
    }
}