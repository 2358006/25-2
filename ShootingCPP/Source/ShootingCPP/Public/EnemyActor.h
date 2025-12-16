// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "EnemyActor.generated.h"

class UMaterialInstanceDynamic;

UCLASS()
class SHOOTINGCPP_API AEnemyActor : public AActor
{
	GENERATED_BODY()

public:
	AEnemyActor();

protected:
	virtual void BeginPlay() override;

public:
	virtual void Tick(float DeltaTime) override;

	UPROPERTY(EditAnywhere)
	class UBoxComponent *boxComp;

	UPROPERTY(EditAnywhere)
	class UStaticMeshComponent *meshComp;

	UPROPERTY(EditAnywhere)
	int32 traceRate = 50;

	UPROPERTY(EditAnywhere)
	float moveSpeed = 800;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "EnemyAppearance")
	FLinearColor InitialColor = FLinearColor(0.1f, 1.0f, 0.1f);

private:
	FVector dir;

	UPROPERTY()
	class UMaterialInstanceDynamic *DynamicMaterialInstance; // 런타임에 머티리얼 파라미터 값을 변경하기 위한 변수 추가

	UFUNCTION()
	void OnEnemyOverlap(UPrimitiveComponent *OverlappedComponent, AActor *OtherActor,
						UPrimitiveComponent *OtherComp, int32 OtherBodyIndex, bool bFromSweep,
						const FHitResult &SweepResult);
};
