# CyberPunk Rider Waifu

사이버펑크 세계관의 3D 액션 RPG 게임입니다. Unity 엔진으로 개발되었으며, VRM 아바타와 스킬 시스템을 특징으로 합니다.

## 🎮 게임 개요

**CyberPunk Rider Waifu**는 사이버펑크 세계를 배경으로 한 3D 액션 게임입니다. 플레이어는 강력한 사이버 와이프와 함께 다양한 적들과 전투를 벌이며, 스킬과 장비를 업그레이드하여 더욱 강해질 수 있습니다.

### 주요 특징

- 🤖 **VRM 아바타 시스템**: VRM 포맷을 지원하는 3D 캐릭터 시스템
- ⚔️ **액션 전투 시스템**: 공격, 스킬, 궁극기 등 다양한 전투 액션
- 🏢 **다양한 스테이지**: 로비, 감옥, 던전 등 여러 환경
- 🔧 **장비 시스템**: 무기 및 장비 업그레이드
- 🎯 **AI 적군 시스템**: 일반 몹부터 엘리트, 보스까지 다양한 적 AI

## 🛠️ 기술 스택

- **Engine**: Unity 2023.3+
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

## 📁 프로젝트 구조

```
Assets/
├── 01.Scenes/          # 게임 씬들
│   ├── KBH_Lobby.unity       # 메인 로비
│   ├── Loading.unity         # 로딩 화면
│   └── Prison_KKH.unity      # 감옥 스테이지
├── 02.Scripts/         # 게임 스크립트
│   ├── Core/              # 핵심 시스템
│   ├── Player/            # 플레이어 관련
│   ├── Enemies/           # 적 AI 시스템
│   ├── UI/               # 사용자 인터페이스
│   ├── Inventory/        # 인벤토리 시스템
│   └── Manager/          # 각종 매니저들
├── Feel/               # Feel 피드백 시스템
├── VRM/               # VRM 캐릭터 시스템
└── CyberPunkGirl&Vehicle/ # 메인 캐릭터 에셋
```

## 🎯 주요 시스템

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

## 🚀 실행 방법

1. **Unity Hub 설치**: Unity Hub를 설치합니다
2. **Unity 버전**: Unity 2023.3 이상 버전으로 프로젝트를 엽니다
3. **패키지 설치**: Package Manager에서 누락된 패키지들을 자동으로 설치합니다
4. **씬 실행**: `Assets/01.Scenes/Loading.unity` 씬부터 시작합니다

### 시스템 요구사항
- Unity 2023.3+
- DirectX 11 이상
- 4GB 이상 RAM 권장

## 🎮 조작법

- **이동**: WASD 또는 화살표 키
- **공격**: 마우스 좌클릭
- **스킬**: 1, 2, 3, 4 키
- **궁극기**: R 키
- **점프**: 스페이스바
- **회피**: Shift 키

## 📋 개발 상태

현재 개발 중인 프로젝트로, 다음 기능들이 구현되어 있습니다:

- ✅ 기본 플레이어 움직임 및 전투
- ✅ 적군 AI 시스템 (일반/엘리트/보스)
- ✅ 기본 UI 시스템
- ✅ VRM 캐릭터 지원
- ✅ 사운드 시스템
- 🚧 인벤토리 시스템 (개발 중)
- 🚧 스킬 시스템 확장 (개발 중)

## 🤝 기여하기

이 프로젝트에 기여하고 싶으시다면:

1. 이 저장소를 포크합니다
2. 새로운 브랜치를 생성합니다 (`git checkout -b feature/amazing-feature`)
3. 변경사항을 커밋합니다 (`git commit -m 'Add amazing feature'`)
4. 브랜치에 푸시합니다 (`git push origin feature/amazing-feature`)
5. Pull Request를 생성합니다

## 📝 라이선스

이 프로젝트는 개인 학습용 프로젝트입니다.

---

**개발자**: CyberPunk Team  
**엔진**: Unity 2023.3+  
**장르**: 3D 액션 RPG  
**플랫폼**: PC (Windows)