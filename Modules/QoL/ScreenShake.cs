namespace GodhomeQoL.Modules.QoL;

public sealed class ScreenShake : Module
{
    public override bool DefaultEnabled => false;

    private protected override void Load()
    {
        USceneManager.activeSceneChanged += OnSceneChanged;
        ApplyShakeState(allowShake: false);
    }

    private protected override void Unload()
    {
        USceneManager.activeSceneChanged -= OnSceneChanged;
        ApplyShakeState(allowShake: true);
    }

    private static void OnSceneChanged(Scene current, Scene next) => ApplyShakeState(allowShake: false);

    private static void ApplyShakeState(bool allowShake)
    {
        GameCameras? cameras = GameCameras.instance;
        if (cameras == null)
        {
            return;
        }

        if (cameras.cameraShakeFSM != null)
        {
            cameras.cameraShakeFSM.enabled = allowShake;
        }

        if (allowShake)
        {
            cameras.ResumeCameraShake();
        }
        else
        {
            cameras.StopCameraShake();
        }
    }
}
