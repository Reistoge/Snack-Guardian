using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void onInteract(IInteractor interactor);
}

public interface IInteractor
{
    void applyEffect(ObjectEffect effect);
}
public interface ISnackSpawner
{
    public Snack releaseSnack();
    public void multiplyLeaveDurationSpeedFactor(float newSpeedFactor);
    void addSnack(int indexConfig);


}
public interface IPlatform
{

}
public interface ITray
{
    public void releaseSnack();
    public void destroyPlatform();
    public void repairPlatform();
    public void setTrayId(string id);
    public string getTrayId();
    TrayConfig getTrayConfig();
    SnackSpawner getSnackSpawnerLoaded();
    bool hasSnacksAvailable();

}
 