using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField]
    private ObstacleBase[] obstacleBasePrefabs;   // TODO ステージのデータが完成したら、そちらから参照する

    [SerializeField] private Tilemap tileMapCollision;

    [SerializeField]
    private Transform obstacleTran;

    // 並べる数
    [SerializeField] private int row;      // 行/ 水平(横)方向
    [SerializeField] private int column;   // 列/ 垂直(縦)方向


    public List<ObstacleBase> GenerateRandomObstacles(int[] weights, int generateCount, StageManager_Presenter stageManager) {  // TODO 引数で生成する情報を受け取る

        List<ObstacleBase> obstaclesList = new List<ObstacleBase>();

        int totalWeight = weights.Sum();
        List<ObstacleBase> goalList = new List<ObstacleBase>();

        for (int i = -row + 1; i < row -1; i++) {
            for (int j = -column + 1; j < column - 1;j++) {

                // プレイヤーのスタート地点の場合、処理しない
                if (i == 0 && j == 0) {
                    continue;
                }

                // タイルマップの座標に変換
                Vector3Int tilePos = tileMapCollision.WorldToCell(new Vector3(i, j, 0));

                // タイルの ColliderType が Grid ではないか確認
                if (tileMapCollision.GetColliderType(tilePos) == Tile.ColliderType.Grid) {
                    // Grid の場合には、通行不可タイルなので処理しない
                    continue;
                }

                if (Random.Range(0, row * column * 4) > row * column * 4 / (row + column)) {
                    continue;
                }

                int index = 0;
                int randomValue = Random.Range(0, totalWeight);

                // 重み付けされた中から生成する障害物を決定
                for (int x = 0; x < obstacleBasePrefabs.Length; x++) {
                    if (randomValue < weights[x]) {
                        index = x;
                        break;
                    }
                    randomValue -= weights[x];
                }

                ObstacleBase obstacle = Instantiate(obstacleBasePrefabs[index], obstacleTran);
                obstacle.transform.position = new Vector3(i, j, 0);
                obstacle.SetUpObstacleBase(ObstacleBase.ObstacleState.Stop, stageManager);
                obstaclesList.Add(obstacle);

                // 移動しない障害物はゴール地点の候補とする
                if (obstacle.obstacleType == ObstacleType.Immovable) {
                    goalList.Add(obstacle);
                }

                generateCount--;

                break;
                //if (generateCount >= weights.Length) {
                //    break;
                //}
            }
            if (generateCount <= 0) {
                break;
            }
        }

        // ゴール用障害物設定。破壊するとゴール地点が生成される
        goalList[Random.Range(0, goalList.Count)].isGoal = true;

        return obstaclesList;
    }
}
