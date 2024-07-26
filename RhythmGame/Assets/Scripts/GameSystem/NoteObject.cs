using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public Note note;               //노트 정보
    public float speed;             //노트 이동 속도
    public float hitPosition;       //판정 위치
    public float startTime;         //게임 시작 시간

    //노트 오브젝트 초기화 
    public void Initialize(Note note, float speed, float hitPosition, float startTime)
    {
        this.note = note;
        this.speed = speed;
        this.hitPosition = hitPosition;
        this.startTime = startTime;

        //노트의 초기 위치 설정
        float initialDistance = speed * (note.startTime - (Time.time - startTime));
        transform.position = new Vector3(hitPosition + initialDistance, note.trackIndex * 2, 0);
    }



    // Update is called once per frame
    void Update()
    {
        //노트 이동
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        
        //판정 위치를 지나면 파괴
        if(transform.position.x < hitPosition - 1)
        {
            Destroy(gameObject);
        }

    }
}
