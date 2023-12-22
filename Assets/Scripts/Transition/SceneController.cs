using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private GameObject _player;
    private NavMeshAgent _playerAgent;
    public GameObject playerPrefab;
    protected override void Awake()
    {
        base.Awake();
        //避免类在新场景加载中被销毁，导致其后的流程无法进行
        DontDestroyOnLoad(this);
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    //TODO:这协程有什么用？执行一次即结束
    //用传送门的Destination Tag 匹配下面其下挂载的Tag
    private IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            //TODO:保存当前玩家状态，给新生成的玩家对象赋值
            //执行完成 yield return 后的函数后继续向下，协程特征
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);
            yield break;
        }
        _player = GameManager.Instance.playerStats.gameObject;
        //控制Agent避免传送时更改坐标导致的瞬移
        _playerAgent = _player.GetComponent<NavMeshAgent>();
        _playerAgent.enabled = false;
        _player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,
            GetDestination(destinationTag).transform.rotation);
        _playerAgent.enabled = true;
        yield return null;
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {  
        var entrances = FindObjectsByType<TransitionDestination>(FindObjectsSortMode.None);
        foreach (var t in entrances)
        {
            if (t.destinationTag == destinationTag)
            {
                return t;
            }
        }

        return null;
    }
}
