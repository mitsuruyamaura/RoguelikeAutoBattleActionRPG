using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.AI;

public class NavigationEnemy : ObstacleBase
{
    [SerializeField]
    private SphereCollider sphereCollider;

    private NavMeshAgent agent;
    private PlayerController player;


    void Start()
    {
        
        if(TryGetComponent(out agent)) {
            sphereCollider.OnTriggerStay2DAsObservable()
                .Where(_ => cururentObstacleState == ObstacleState.Stop)
                .Where(_ => player == null)
                .Where(other => other.TryGetComponent(out player))
                .Subscribe(other => 
                {
                    agent.SetDestination(player.transform.position);
                    cururentObstacleState = ObstacleState.Move;
                })
                .AddTo(this);

            sphereCollider.OnTriggerExit2DAsObservable()
                .Where(_ => cururentObstacleState == ObstacleState.Move)
                .Where(_ => player != null)
                .Subscribe(other => {
                    agent.ResetPath();
                    cururentObstacleState = ObstacleState.Stop;
                })
                .AddTo(this);
        }


        player!.transform.ObserveEveryValueChanged(x => x.position)
            .Subscribe(vec3 => agent.SetDestination(vec3));
    }




    
}
