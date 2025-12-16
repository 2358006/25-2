// Fill out your copyright notice in the Description page of Project Settings.

#include "EnemyActor.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "EngineUtils.h"
#include "PlayerPawn.h"
#include "ShootingGameModeBase.h"
#include "Materials/MaterialInstanceDynamic.h" // 런타임 색상 변경을 위해 필요한 헤더

AEnemyActor::AEnemyActor()
{
	PrimaryActorTick.bCanEverTick = true;

	boxComp = CreateDefaultSubobject<UBoxComponent>(TEXT("Box Collider"));
	SetRootComponent(boxComp);
	boxComp->SetBoxExtent(FVector(50.0f, 50.0f, 50.0f));

	meshComp = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Static mesh"));
	meshComp->SetupAttachment(boxComp);

	// Collision presets을 Enemy 프리셋으로 변경
	boxComp->SetCollisionProfileName(TEXT("Enemy"));
}

void AEnemyActor::BeginPlay()
{
	Super::BeginPlay();

	if (meshComp && meshComp->GetMaterial(0))
	{
		// 런타임에 조작 가능한 Dynamic Material Instance를 생성하고 저장
		DynamicMaterialInstance = meshComp->CreateAndSetMaterialInstanceDynamicFromMaterial(0, meshComp->GetMaterial(0));

		if (DynamicMaterialInstance)
		{
			// 이제 InitialColor 변수에 저장된 값을 사용합니다.
			// 이 값은 블루프린트에서 수정될 수 있습니다.
			DynamicMaterialInstance->SetVectorParameterValue(TEXT("BaseColorParam"), InitialColor);
		}
	}

	// 1 ~ 100 사이의 임의의 정수 값 지정
	int32 drawResult = FMath::RandRange(1, 100);

	// 값이 추적 확률 변수보다 작거나 같으면
	if (drawResult <= traceRate)
	{
		// 월드 공간에 APlayerPawn 클래스로 된 액터를 모두 검색
		for (TActorIterator<APlayerPawn> player(GetWorld()); player; ++player)
		{
			// 검색된 액터의 이름에 "BP_PlayerPawn"이란 문구가 포함되어 있으면
			if (player->GetName().Contains(TEXT("BP_PlayerPawn")))
			{
				// 플레이어 액터의 위치 - 자신의 위치
				dir = player->GetActorLocation() - GetActorLocation();
				dir.Normalize();
			}
		}
	}
	// 그렇지 않다면 정면 방향 벡터를 생성
	else
	{
		dir = GetActorForwardVector();
	}

	// 박스 컴포넌트의 BeginOverlap 델리게이트에 OnEnemyOverlap 함수를 연결.
	boxComp->OnComponentBeginOverlap.AddDynamic(this, &AEnemyActor::OnEnemyOverlap);
}

void AEnemyActor::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	// BeginPlay()에서 결정된 방향으로 이동
	FVector newLocation = GetActorLocation() + dir * moveSpeed * DeltaTime;
	SetActorLocation(newLocation);
}

void AEnemyActor::OnEnemyOverlap(UPrimitiveComponent *OverlappedComponent, AActor *OtherActor, UPrimitiveComponent *OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult &SweepResult)
{
	// 충돌한 대상 액터를 APlayerPawn 클래스로 변환을 시도
	APlayerPawn *player = Cast<APlayerPawn>(OtherActor);

	// 캐스팅이 성공하면
	if (player != nullptr)
	{
		// 부딪힌 대상 액터를 제거
		OtherActor->Destroy();

		// 현재 게임 모드를 가져옴
		AShootingGameModeBase *currentGameMode = Cast<AShootingGameModeBase>(GetWorld()->GetAuthGameMode());

		if (currentGameMode != nullptr)
		{
			// 메뉴 UI 생성 함수를 호출
			currentGameMode->ShowMenu();
		}
	}

	// 자기 자신을 제거
	Destroy();
}
