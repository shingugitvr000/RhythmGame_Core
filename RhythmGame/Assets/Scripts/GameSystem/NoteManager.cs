using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public AudioClip audioClip;                         //재생할 오디오 클립
    public List<Note> notes= new List<Note>();          //모든 노트 정보를 담는 리스트 
    public float bpm = 120f;                            //곡의 BPM
    public float speed = 1f;                            //재생 속도
    public GameObject notePrefabs;                      //노트 프리팹

    public float audioLatency = 0.1f;                   //오디오 지연 시간 
    public float hitPosition = -8.0f;                   //노트 판정 위치
    public float noteSpeed = 10;                        //노트 이동 속도

    private AudioSource audioSource;                    //오디오 소스 콤포넌트
    private float startTime;                            //게임 시작 시간
    private List<Note> activeNotes = new List<Note>();  //아직 생성 되지 않은 노트 리스트 
    private float spawnOffset;                          //노드 생성 시간 오프셋

    public bool debugMode = false;                      //디버그 모드 플래그
    public GameObject hitPositionMarker;                //판정 위치 마커 오브젝트

    public float initialDelay = 3f;                     //초기 지연시간


    //게임 초기화
    public void Initialize()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        startTime = Time.time + initialDelay;               //시작 시간을 지연 시간 만큼 미룸
        activeNotes.Clear();                                //List 사용시 초기화 clear 해주는것이 좋음
        activeNotes.AddRange(notes);
        spawnOffset = (10 - hitPosition) / noteSpeed;       //노트 생성 시간 오프셋 계산

        if (debugMode)
        {
            CreateHitPositionMarker();
        }

        StartCoroutine(StartAudioWithDelay());              //지연 후 오디오 재생 코루틴 시작

    }

    //지연 후 오디오 재생을 위한 코루틴
    private IEnumerator StartAudioWithDelay()
    {
        yield return new WaitForSeconds(initialDelay);
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time - startTime;          //현재 게임 시간을 계산

        //활성화된 노트를 처리 
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            Note note = activeNotes[i];
            if(currentTime >= note.startTime - spawnOffset && currentTime < note.startTime + note.duration)
            {
                SpwanNoteObject(note);
                activeNotes.RemoveAt(i);
            }
            else if(currentTime >= note.startTime + note.duration)
            {
                activeNotes.RemoveAt(i);
            }
        }
    }

    //새로운 노트 추가 
    public void AddNote(Note note)
    {
        notes.Add(note);
    }

    //재생 속도 설정
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;   
    }

    //노트 오브젝트 생성 
    private void SpwanNoteObject(Note note)
    {
        GameObject noteObject = Instantiate(notePrefabs, new Vector3(10, note.trackIndex * 2, 0), Quaternion.identity);
        noteObject.GetComponent<NoteObject>().Initialize(note, noteSpeed, hitPosition, startTime);  //생성 하면서 데이터 초기화
    }
    
    //오디오 지연 시간 조정
    public void AdjustAudioLatency(float latency)
    {
        audioLatency = latency;
    }

    //디버그용 판정 위치 마커 생성 
    private void CreateHitPositionMarker()
    {
        hitPositionMarker = GameObject.CreatePrimitive(PrimitiveType.Cube);             //기본 도형을 생성 한다.
        hitPositionMarker.transform.position = new Vector3(hitPosition, 0, 0);          //hit 위치로 이동 시킨다.
        hitPositionMarker.transform.localScale = new Vector3(0.1f, 10.0f, 1.0f);        //스캐일 값을 조절하여 선을 만든다. 
    }
}
