using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField]
    private ObstacleBase[] obstacleBasePrefabs;   // TODO �X�e�[�W�̃f�[�^������������A�����炩��Q�Ƃ���

    [SerializeField] private Tilemap tileMapCollision;

    [SerializeField]
    private Transform obstacleTran;

    // ���ׂ鐔
    [SerializeField] private int row;      // �s/ ����(��)����
    [SerializeField] private int column;   // ��/ ����(�c)����


    public List<ObstacleBase> GenerateRandomObstacles(int[] weights, int generateCount, StageManager_Presenter stageManager) {  // TODO �����Ő�����������󂯎��

        List<ObstacleBase> obstaclesList = new List<ObstacleBase>();

        int totalWeight = weights.Sum();
        List<ObstacleBase> goalList = new List<ObstacleBase>();

        for (int i = -row + 1; i < row -1; i++) {
            for (int j = -column + 1; j < column - 1;j++) {

                // �v���C���[�̃X�^�[�g�n�_�̏ꍇ�A�������Ȃ�
                if (i == 0 && j == 0) {
                    continue;
                }

                // �^�C���}�b�v�̍��W�ɕϊ�
                Vector3Int tilePos = tileMapCollision.WorldToCell(new Vector3(i, j, 0));

                // �^�C���� ColliderType �� Grid �ł͂Ȃ����m�F
                if (tileMapCollision.GetColliderType(tilePos) == Tile.ColliderType.Grid) {
                    // Grid �̏ꍇ�ɂ́A�ʍs�s�^�C���Ȃ̂ŏ������Ȃ�
                    continue;
                }

                if (Random.Range(0, row * column * 4) > row * column * 4 / (row + column)) {
                    continue;
                }

                int index = 0;
                int randomValue = Random.Range(0, totalWeight);

                // �d�ݕt�����ꂽ�����琶�������Q��������
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

                // �ړ����Ȃ���Q���̓S�[���n�_�̌��Ƃ���
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

        // �S�[���p��Q���ݒ�B�j�󂷂�ƃS�[���n�_�����������
        goalList[Random.Range(0, goalList.Count)].isGoal = true;

        return obstaclesList;
    }
}
