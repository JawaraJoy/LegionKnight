using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random=UnityEngine.Random;


public class CubeSpawner : MonoBehaviour
{
    public GameObject Spawner;
    [SerializeField]
    MovingCubeArahKanan cubePrefabKiri;

    [SerializeField]
    MovingCubeArahKiri cubePrefabKanan;

    public GameObject[] ObstacleKiri;
    public GameObject[] ObstacleKanan;

    public GameObject[] RandomObstacleKiri;
    public GameObject[] RandomObstacleKanan;

    public Transform Posisi; 

    public int Mode, Attack;

    public int ScoreBerapaArrowDatang, RandomObstacleScore;

    public bool isTraining;
    
    //public GameObject SkillObj;
    /*public void SpawnCube(){
        var cube = Instantiate(cubePrefabKiri);
        cube.transform.position = transform.position;
    }*/

    /*private void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, cubePrefabKiri.transform.localScale);
    }*/

    //public GameObject CameraPlayer;

    public void TambahPosition(){
        
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        pos.x = Random.Range(0, 2) == 0 ? -3.32f : 3.32f;
        transform.position = pos;

        /*Vector3 CameraPlayer = transform.position;
        CameraPlayer.y += 0.5f;
        transform.position = CameraPlayer;*/
        
        if (Mode == 0){
        //Untuk Classic Mode
        //Random X
            /*if (Spawner.transform.position.x == -3.32f){

                //Move Speed
                //MovingCubeArahKanan.moveSpeed = 3;

                //Spawn Cube
                var cube = Instantiate(cubePrefabKiri);
                cube.transform.position = transform.position;
            }
            if (Spawner.transform.position.x == 3.32f){

                //MoveSpeed
                //MovingCubeArahKanan.moveSpeed = -3;

                var cube = Instantiate(cubePrefabKanan);
                cube.transform.position = transform.position;
            }*/
            if (GameManager.Scores < RandomObstacleScore){
                CubeSpawnerFunction();
            }
            if (GameManager.Scores >= RandomObstacleScore){
                RandomObstacleFunction();
            }

        }

        if (Mode == 1){
            //Untuk Story Mode
            if (GameManager.Scores < ScoreBerapaArrowDatang){
                CubeSpawnerFunction();
            }

            if (GameManager.Scores >= ScoreBerapaArrowDatang){
                CubeSpawnerBoss();
            }
        }
    }

    void Start(){
        //SkillObj = GameObject.FindGameObjectWithTag("Skills");
        //InvokeRepeating("SpawnCube", 1.0f, 2f);
        InvokeRepeating("TambahPosition", 1.0f, 1.2f);
    }

    public void GameOverCancelInvoke()
    {
        CancelInvoke();
        Vector3 pos = transform.position;
        pos.y += -0.5f;
        transform.position = pos;
    }

    public void StopObstacleInTutorial(){
        CancelInvoke();
        Vector3 pos = transform.position;
        //pos.y += 0.5f;
        transform.position = pos;
    }

    //Untuk mode Story//////////////////////////////////////////
    public void StopSpawnObs(){
        CancelInvoke();
        Vector3 pos = transform.position;
        pos.y += -0.5f;
        transform.position = pos;
    }

    public void StopSpawnObsRocket(){
        CancelInvoke();
        Vector3 pos = transform.position;
        //pos.y += 0.5f;
        transform.position = pos;
    }

    public void StartSpawnObs(){
        InvokeRepeating("TambahPosition", 1.0f, 1.2f);
    }

    public void StartRocket(){
        InvokeRepeating("TambahPosition", 0.13f, 0.13f);
    }
    public void RocketOff(){
        InvokeRepeating("TambahPosition", 1f, 1.2f);
    }


    //Untuk mode Story//////////////////////////////////////////////

    void Update(){
        /*Attack = PlayerPrefs.GetInt("Attack");
        if (Attack == 1){
            StartSpawnObs();
        }*/
        /*if (SkillObj.GetComponent<Skills>().Rocket = true){
            InvokeRepeating("TambahPosition", 2.0f, 2.2f);
        }*/
        Mode = PlayerPrefs.GetInt("ModeGamePlay");
    }

    public bool temp = false;
    public void GameOverStartInvoke()
    {
        if(!temp)
        {
            InvokeRepeating("TambahPosition", 1.0f, 1.2f);
            temp = true;
        }    
    }


    //CubeSpawner
    public void CubeSpawnerFunction(){
        //Random X
        if (Spawner.transform.position.x == -3.32f){

            //Move Speed
            //MovingCubeArahKanan.moveSpeed = 3;

            //Spawn Cube
            var cube = Instantiate(cubePrefabKiri);
            cube.transform.position = transform.position;
        }
        if (Spawner.transform.position.x == 3.32f){

            //MoveSpeed
            //MovingCubeArahKanan.moveSpeed = -3;

            var cube = Instantiate(cubePrefabKanan);
            cube.transform.position = transform.position;
        }
    }

    public void CubeSpawnerBoss(){
        //Random Obstacle
        if (Spawner.transform.position.x == -3.32f){

            //Move Speed
            //MovingCubeArahKanan.moveSpeed = 3;

            //Spawn Cube
            int n = Random.Range(0, ObstacleKiri.Length);
            Instantiate(ObstacleKiri[n], Posisi.position, ObstacleKiri[n].transform.rotation);
        }
        if (Spawner.transform.position.x == 3.32f){

            //MoveSpeed
            //MovingCubeArahKanan.moveSpeed = -3;

            int n = Random.Range(0, ObstacleKanan.Length);
            Instantiate(ObstacleKanan[n], Posisi.position, ObstacleKanan[n].transform.rotation);
        }
    }
    
    //RandomObstacleTanpaArrow////////////////////////////////////////////////////////////////////////////////////////////
    public void RandomObstacleFunction(){
        //Random Obstacle
        if (Spawner.transform.position.x == -3.32f){

            //Move Speed
            //MovingCubeArahKanan.moveSpeed = 3;

            //Spawn Cube
            int n = Random.Range(0, RandomObstacleKiri.Length);
            Instantiate(RandomObstacleKiri[n], Posisi.position, RandomObstacleKiri[n].transform.rotation);
        }
        if (Spawner.transform.position.x == 3.32f){

            //MoveSpeed
            //MovingCubeArahKanan.moveSpeed = -3;

            int n = Random.Range(0, RandomObstacleKanan.Length);
            Instantiate(RandomObstacleKanan[n], Posisi.position, RandomObstacleKanan[n].transform.rotation);
        }
    }
}
