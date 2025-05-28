using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : Singleton<SceneMover>
{


    public void ShowDungeonPopup(string dungeonName)
    {
        //UIManager.Instance.PopupManager.PopupStack.Clear();
        
        
        UIManager.Instance.PopupManager.ShowAnswerPopup("배달하러 가시겠습니까?","당근 빳따죠!","아, 잠만요",() =>
        {
            UIManager.Instance.PopupManager.CloseAllPopups();
            
            //이제 던전에 들어가고 커서 락이 필요함
            UIManager.Instance.isInDelivery = true;
            UIManager.Instance.isCursorLockNeed = true;
            
            SceneManager.LoadScene(dungeonName);
           
        },numberofbuttons : 2);

    }
    
    
    public void MovetoScene(string sceneName)
    {
        
        UIManager.Instance.PopupManager.PopupStack.Clear();
        SceneManager.LoadScene(sceneName);

    }
}
