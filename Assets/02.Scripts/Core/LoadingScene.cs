using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
   public string targetSceneName;
   
   
      
      private AsyncOperation asyncOperation;
      private bool isReadyToActivate = false;

      void Start()
      {

         targetSceneName = SceneMover.Instance.TargetSceneName;
         
         Debug.Log($"[시작] 현재 활성화된 씬 이름: {SceneManager.GetActiveScene().name}");
         StartCoroutine(LoadSceneAsync());
      }

      IEnumerator LoadSceneAsync()
      {
         Debug.Log($"[{Time.time}] '{targetSceneName}' 씬 비동기 로딩 시작");
         asyncOperation = SceneManager.LoadSceneAsync(targetSceneName);
         asyncOperation.allowSceneActivation = false;

         while (asyncOperation.progress < 0.9f)
         {
            Debug.Log($"[{Time.time}] 로딩 중... 진행도: {asyncOperation.progress * 100}%");
            yield return null;
         }

         Debug.Log($"[{Time.time}] 로딩 완료! 스페이스바 입력을 기다립니다.");
         isReadyToActivate = true;
      }

      void Update()
      {
         // 현재 씬 이름 지속 출력 (디버깅용)
         // if (Time.frameCount % 60 == 0)
         // {
         //    Debug.Log($"[{Time.time}] 현재 활성화된 씬: {SceneManager.GetActiveScene().name}");
         // }

         // 스페이스바 입력 시 씬 전환
         if (isReadyToActivate && Input.GetKeyDown(KeyCode.Space))
         {
            Debug.Log($"[{Time.time}] 스페이스바 입력 확인, 씬 전환 시작!");
            Skip();
         }
      }


      public void Skip()
      {
         asyncOperation.allowSceneActivation = true;
      }
      
   }


