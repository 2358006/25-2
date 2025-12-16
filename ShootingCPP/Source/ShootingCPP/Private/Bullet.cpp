// Fill out your copyright notice in the Description page of Project Settings.

#include "Bullet.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "EnemyActor.h"
#include "Kismet/GameplayStatics.h"
#include "ShootingGameModeBase.h"

ABullet::ABullet()
{
	PrimaryActorTick.bCanEverTick = true;

	boxComp = CreateDefaultSubobject<UBoxComponent>(TEXT("Box Collider"));
	SetRootComponent(boxComp);
	boxComp->SetBoxExtent(FVector(50.0f, 50.0f, 50.0f));

	// 박스 컴포넌트의 크기 변경
	boxComp->SetWorldScale3D(FVector(0.75f, 0.25f, 1.0f));

	meshComp = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Static Mesh Component"));
	meshComp->SetupAttachment(boxComp);

	// 박스 컴포넌트의 콜리전 프리셋을 Bullet으로 설정.
	boxComp->SetCollisionProfileName(TEXT("Bullet"));
}

void ABullet::BeginPlay()
{
	Super::BeginPlay();

	// 박스 컴포넌트의 충돌 오버랩 이벤트에 BulletOverlap 함수 연결
	boxComp->OnComponentBeginOverlap.AddDynamic(this, &ABullet::OnBulletOverlap);
}

void ABullet::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	// 전방으로 이동될 위치를 계산
	FVector newLocation = GetActorLocation() + GetActorForwardVector() * moveSpeed * DeltaTime;

	// 계산된 위치 좌표를 액터의 새 좌표로 넣음
	SetActorLocation(newLocation);
}

// 충돌 이벤트가 발생할 때 실행
void ABullet::OnBulletOverlap(UPrimitiveComponent *OverlappedComponent, AActor *OtherActor, UPrimitiveComponent *OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult &SweepResult)
{
	// 충돌한 액터를 AEnemyActor 클래스로 변환
	AEnemyActor *enemy = Cast<AEnemyActor>(OtherActor);

	// 캐스팅이 정상적으로 되어서 AEnemyActor 포인터 변수에 값에 있으면
	if (enemy != nullptr)
	{
		// 충돌한 액터를 제거
		OtherActor->Destroy();

		// 폭발 이펙트를 생성
		UGameplayStatics::SpawnEmitterAtLocation(GetWorld(), explosionFX, GetActorLocation(), GetActorRotation());

		// 현재 게임 모드를 가져옴
		AGameModeBase *currentMode = GetWorld()->GetAuthGameMode();

		// AShootingGameModeBase 클래스로 변환
		AShootingGameModeBase *currentGameModeBase = Cast<AShootingGameModeBase>(currentMode);

		// 게임 모드 베이스를 가져오면
		if (currentGameModeBase != nullptr)
		{
			// 게임 모드 베이스의 점수를 1점 추가
			currentGameModeBase->AddScore(1);
		}
	}

	// 자기 자신 제거
	Destroy();
}
