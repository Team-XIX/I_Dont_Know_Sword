# 📝 게임 소개
<img src="https://github.com/Team-XIX/I_Dont_Know_Sword/blob/main/knightnosword.PNG?raw=true" width="800">

- 프로젝트명 : 기사가 검술을 모름
- 게임 컨셉 : 2D 로그라이크 탑다운 슈팅게임
- 레퍼런스 : 건전, 바인딩 오브 아이작
- 개발 환경 : Unity
- 제작 기간 : 3/27 ~ 4/2
- itch.io 주소 : [웹 게임 실행](https://punksoda.itch.io/i-dont-know-sword)
      Play game을 누른뒤 우측 하단의 파란색 창 버튼 클릭으로 큰화면 플레이 가능

<br />

## Team Members (팀원 및 팀 소개)
| 최시훈 | 김민준 | 정순원 | 서상원 | 김준혁 |
|:------:|:------:|:------:|:------:|:------:|
| UI/사운드/이펙트 | 몬스터 | 플레이어 | 데이터 | 스테이지 |
| [GitHub](https://github.com/Punksoda) | [GitHub](https://github.com/Toaaaa) | [GitHub](https://github.com/jsw981117) | [GitHub](https://github.com/sangweon25) | [GitHub](https://github.com/chajungto) |


<br />

## ⌨️ 조작법

| 조작 | 키 |
|------|----|
| 이동 | `W A S D` |
| 구르기 | `Space` |
| 시점 및 조준 | `마우스` |
| 공격 | `마우스 좌클릭` |
| 무기 변경 | `Q` |


<br />


### 📌 기능 구현
- 기본 UI : 플레이어 체력, 미니맵, 무기 및 스킬 표시
- 플레이어 동작 : 키입력을 통한 동작과 특수 동작 (구르기) 구현
- 몬스터 행동 : FSM을 통해 몬스터의 상태를 조절
- 레벨 디자인 : 절차적 맵 생성을 통한 프리셋의 맵을 연속적으로 배치
- 간단한 몬스터 상태 : 간단한 FSM을 통해 행동의 뼈대 제작
- 전투 : 플레이어의 무기와 몬스터의 발사체 기능 구현 
- 데이터 관리 : 구글 스프레드 시트 데이터를 SO와 Json으로 로드



<br />

## ⚙ 기술 스택
- C#
- Unity
- Git
- Notion
    - https://www.notion.so/teamsparta/XIX-1bc2dc3ef51481b1ad71c57ba7f25663
- Figma
    - https://www.figma.com/board/EJCyvfUO6CekCFPJFRw7f7/%EA%B8%B0%EC%82%AC%EA%B0%80%EA%B2%80%EC%88%A0%EC%9D%84%EB%AA%A8%EB%A6%84?node-id=0-1&p=f&t=hqKEhFOzDejAgptg-0


<br />

## 🤔 트러블 슈팅 및 핵심 기능 
- 몬스터 벽에 걸림 방지
    - [Physics2D.OverlapPoint - 2D 상의 겹침의 정도](https://toacode.tistory.com/44)
 
- 카메라 이동 시 스프라이트가 흔들리는 것처럼 보이는 현상
    - [FixedUpdate - 카메라 움직임](https://tnjs0104.tistory.com/68)

- HP_Container Null 레퍼런스 참조 오류
    - [이벤트 구독, 취소 null 레퍼런스](https://github.com/Punksoda/TIL/blob/main/Unity_2025_04_02.md)


<br />
