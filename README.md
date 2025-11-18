# CyberPunk Rider

사이버펑크 세계관의 3D 액션 RPG 게임입니다. Unity 엔진으로 개발되었으며, VRM 아바타와 스킬 시스템을 특징으로 합니다.

## 게임 개요

**CyberPunk Rider**는 사이버펑크 세계를 배경으로 한 3D 액션 게임입니다. 삼국무쌍 스타일의 시원한 무쌍 액션을 기반으로 하며, 미소녀 캐릭터를 중심으로 한 연출과 스타일리시한 전투를 강조하였습니다. 다양한 컷신을 통해 시각적 재미와 몰입도를 함께 높이고자 했으며, 플레이어는 강력한 사이버 와이프와 함께 다양한 적들과 전투를 벌이며 스킬과 장비를 업그레이드하여 더욱 강해질 수 있습니다.

### 주요 특징

- **VRM 아바타 시스템**: VRM 포맷을 지원하는 3D 캐릭터 시스템
- **액션 전투 시스템**: 공격, 스킬, 궁극기 등 다양한 전투 액션
- **다양한 스테이지**: 로비, 감옥, 던전 등 여러 환경
- **장비 시스템**: 무기 및 장비 업그레이드
- **AI 적군 시스템**: 일반 몹부터 엘리트, 보스까지 다양한 적 AI

## 기술 스택

- **Engine**: Unity 6000.0.48f1
- **Rendering**: Universal Render Pipeline (URP)
- **Character System**: VRM 1.0 / VRM 0.x
- **Audio**: Unity Audio System + Cinemachine
- **Input**: Unity Input System
- **AI Navigation**: Unity AI Navigation

### 주요 Unity 패키지

- Unity Input System (1.13.1)
- Cinemachine (3.1.3)
- Universal Render Pipeline (17.0.4)
- VRM/UniVRM - VRM 캐릭터 지원
- Feel - 피드백 시스템
- Addressables - 리소스 관리

## 주요 시스템

### 플레이어 시스템
- **PlayerInput.cs**: 입력 처리 (이동, 공격, 스킬)
- **StatComponent.cs**: 캐릭터 능력치 관리
- **PlayerHit.cs**: 피격 처리

### 적군 AI 시스템
- **State Machine 기반 AI**: 상태 기반 적군 AI
- **NormalMonsterAI**: 일반 몬스터 AI
- **EliteMonsterAI**: 엘리트 몬스터 AI  
- **Boss_Waifu_AI**: 보스 AI (다양한 공격 패턴)

### 전투 시스템
- 기본 공격, 스킬 공격 (4개), 궁극기
- 크리티컬 시스템 및 데미지 계산
- 무기별 고유 특성

### UI 시스템
- 인벤토리 및 장비 시스템
- 상점 시스템
- 스킬 UI
- 미니맵

## 실행 방법

1. **Unity Hub 설치**: Unity Hub를 설치합니다
2. **Unity 버전**: Unity 6000.0.48f1 버전으로 프로젝트를 엽니다
3. **패키지 설치**: Package Manager에서 누락된 패키지들을 자동으로 설치합니다
4. **씬 실행**: `Assets/01.Scenes/Loading.unity` 씬부터 시작합니다

### 시스템 요구사항
- Unity 6000.0.48f1
- DirectX 11 이상
- 4GB 이상 RAM 권장

## 게임 플로우

### 전체 게임 진행 순서

```
시작 → 로비 → 던전 선택 → 로딩 → 게임플레이 → 보스전 → 클리어 → 로비
  ▲                                                            │
  │                                                            │
  └────────────── 반복 (더 어려운 던전) ──────────────────────────┘
```

### 주요 스테이지

1. **오프닝 (Opening)**: 게임 시작 화면 및 메뉴
2. **메인 로비 (KBJ_Lobby)**: 캐릭터 관리, 인벤토리, 상점, 스킬 업그레이드
3. **로딩 (Loading)**: 던전 로딩 화면 (스페이스바로 진행)
4. **던전 인트로 (KBJ_DungeonIntro)**: 미션 소개 및 목표 안내
5. **게임플레이 (KBJ_Procedure)**: 실제 전투 및 탐험
6. **보스 등장 (KBJ_Boss1/2Appear)**: 보스 등장 컷씬
7. **던전 클리어 (KBJ_DueongunClear)**: 보상 획득 및 결과

**상세한 게임 플로우 다이어그램**: [GameFlowDiagram.md](GameFlowDiagram.md) 파일을 참고하세요.

## 조작법

- **이동**: WASD 또는 화살표 키
- **공격**: 마우스 좌클릭
- **스킬**: 1, 2, 3, 4 키
- **궁극기**: R 키
- **점프**: 스페이스바
- **회피**: Shift 키

**엔진**: Unity 6000.0.58f2  
**장르**: 3D 액션 RPG  
**플랫폼**: PC (Windows)
