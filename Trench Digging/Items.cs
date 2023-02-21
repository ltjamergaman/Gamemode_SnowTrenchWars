datablock ParticleData(DirtShootParticle)
{
	dragCoefficient = 0.75;
	windCoefficient = 0.2;
	gravityCoefficient = 0.01;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 200;
	lifetimeVarianceMS = 25;
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = true;
	textureName = "base/data/particles/cloud";

	colors[0] = "0.6 0.35 0 1";
	colors[1] = "0.5 0.25 0 1";
	colors[2] = "0.25 0.125 0 0";
	sizes[0] = 0.55;
	sizes[1] = 0.63;
	sizes[2] = 0.4;
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
datablock ParticleEmitterData(DirtShootEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 5;
	ejectionVelocity = 0.75;
	velocityVariance = 0;
	ejectionOffset = 0;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	particles = DirtShootParticle;
	uiName = "Dirt Shot";
};


datablock ProjectileData(TrenchDirtProjectile)
{
	shapeFile = "base/data/shapes/empty.dts";
   directDamage = 0;
   directDamageType = "";
   radiusDamageType = "";

   brickExplosionRadius = 0;
   brickExplosionImpact = false;
   brickExplosionForce = 0;
   brickExplosionMaxVolume = 0;
   brickExplosionMaxVolumeFloating = 0;

   impactImpulse = 0;
   verticalImpulse = 0;
   explosion = "";
   particleEmitter = DirtShootEmitter;

   muzzleVelocity = 40;
   velInheritFactor = 1;

   armingDelay = 0;
   lifetime = 350;
   fadeDelay = 0;
   bounceElasticity = 0.5;
   bounceFriction = 0.20;
   isBallistic = true;
   gravityMod = 0;

   hasLight = false;
   lightRadius = 2;
   lightColor = "1 0.5 0";

   uiName = "Trench Dirt";
};


datablock itemData(TrenchShovelItem)
{
	shapeFile = "base/data/shapes/brickweapon.dts";
	uiName = "Trench Shovel";
	doColorShift = true;
	colorShiftColor = "0.475 0.555 0.475 1.000";
	
	image = "TrenchShovelImage";
	canDrop = true;
};
datablock itemData(TrenchDirtItem)
{
	shapeFile = "base/data/shapes/brickweapon.dts";
	uiName = "Trench Dirt";
	doColorShift = true;
	colorShiftColor = "0.545098 0.270588 0.074509 1";
	
	image = "TrenchDirtImage";
	canDrop = true;
};

datablock ShapeBaseImageData(TrenchShovelImage)
{
	projectile = "";
	shapeFile = "base/data/shapes/brickweapon.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	correctMuzzleVector = false;
	className = "WeaponImage";
	item = TrenchShovelItem;
	ammo = " ";
	melee = true;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.475 0.555 0.475 1.000";

	stateName[0]                   = "Activate";
	stateTimeoutValue[0]           = 0.5;
	stateTransitionOnTimeout[0]    = "Ready";
	stateSound[0]                  = weaponSwitchSound;

	stateName[1]                   = "Ready";
	stateTransitionOnTriggerDown[1]= "PreFire";
	stateAllowImageChange[1]       = true;

	stateName[2]                   = "PreFire";
	stateScript[2]                 = "onPreFire";
	stateAllowImageChange[2]       = false;
	stateTimeoutValue[2]           = 0.1;
	stateTransitionOnTimeout[2]    = "Fire";

	stateName[3]                   = "Fire";
	stateTransitionOnTimeout[3]    = "CheckFire";
	stateTimeoutValue[3]           = 0.3;
	stateFire[3]                   = true;
	stateAllowImageChange[3]       = false;
	stateScript[3]                 = "onFire";
	stateWaitForTimeout[3]         = true;

	stateName[4]                   = "CheckFire";
	stateTransitionOnTriggerUp[4]  = "StopFire";
	stateTransitionOnTriggerDown[4]= "PreFire";

	stateName[5]                   = "StopFire";
	stateTransitionOnTimeout[5]    = "Ready";
	stateTimeoutValue[5]           = 0.2;
	stateAllowImageChange[5]       = false;
	stateWaitForTimeout[5]         = true;
};
datablock ShapeBaseImageData(TrenchDirtImage)
{
	shapeFile = "base/data/shapes/brickweapon.dts";
	emap = true;

	mountPoint = 0;
	offset = "0 0 0";

	correctMuzzleVector = false;

	className = "WeaponImage";
	item = TrenchDirtItem;
	ammo = " ";

	projectile = TrenchDirtProjectile;
	projectileType = Projectile;

	melee = false;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.545098 0.270588 0.074509 1";

	stateName[0]                   = "Activate";
	stateTimeoutValue[0]           = 0.5;
	stateTransitionOnTimeout[0]    = "Ready";
	stateSound[0]                  = weaponSwitchSound;

	stateName[1]                   = "Ready";
	stateTransitionOnTriggerDown[1]= "PreFire";
	stateAllowImageChange[1]       = true;

	stateName[2]                   = "PreFire";
	stateScript[2]                 = "onPreFire";
	stateAllowImageChange[2]       = false;
	stateTimeoutValue[2]           = 0.1;
	stateTransitionOnTimeout[2]    = "Fire";

	stateName[3]                   = "Fire";
	stateTransitionOnTimeout[3]    = "CheckFire";
	stateTimeoutValue[3]           = 0.3;
	stateFire[3]                   = true;
	stateAllowImageChange[3]       = false;
	stateScript[3]                 = "onFire";
	stateWaitForTimeout[3]         = true;

	stateName[4]                   = "CheckFire";
	stateTransitionOnTriggerUp[4]  = "StopFire";
	stateTransitionOnTriggerDown[4]= "PreFire";

	stateName[5]                   = "StopFire";
	stateTransitionOnTimeout[5]    = "Ready";
	stateTimeoutValue[5]           = 0.2;
	stateAllowImageChange[5]       = false;
	stateWaitForTimeout[5]         = true;
};

datablock ShapeBaseImageData(AdminDirtImage)
{
	projectile = "";
	shapeFile = "base/data/shapes/brickweapon.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	correctMuzzleVector = false;
	className = "WeaponImage";
	item = TrenchDirtItem;
	ammo = " ";

	projectile = TrenchDirtProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.545098 0.270588 0.074509 1";

	stateName[0]                   = "Activate";
	stateTimeoutValue[0]           = 0.05;
	stateTransitionOnTimeout[0]    = "Ready";
	stateSound[0]                  = weaponSwitchSound;

	stateName[1]                   = "Ready";
	stateTransitionOnTriggerDown[1]= "Fire";

	stateName[2]                   = "Fire";
	stateTimeoutValue[2]           = 0.04;
	stateTransitionOnTimeout[2]    = "Ready";
	stateFire[2]                   = true;
	stateAllowImageChange[2]       = false;
	stateScript[2]                 = "onFire";
	stateWaitForTimeout[2]         = true;
};
datablock ShapeBaseImageData(AdminShovelImage)
{
	projectile = "";
	shapeFile = "base/data/shapes/brickweapon.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	correctMuzzleVector = false;
	className = "WeaponImage";
	item = TrenchShovelItem;
	ammo = " ";
	melee = true;
	doRetraction = false;
	armReady = true;
	doColorShift = true;
	colorShiftColor = "0.475 0.555 0.475 1.000";

	stateName[0]                   = "Activate";
	stateTimeoutValue[0]           = 0.05;
	stateTransitionOnTimeout[0]    = "Ready";
	stateSound[0]                  = weaponSwitchSound;

	stateName[1]                   = "Ready";
	stateTransitionOnTriggerDown[1]= "Fire";

	stateName[2]                   = "Fire";
	stateTimeoutValue[2]           = 0.04;
	stateTransitionOnTimeout[2]    = "Ready";
	stateFire[2]                   = true;
	stateAllowImageChange[2]       = false;
	stateScript[2]                 = "onFire";
	stateWaitForTimeout[2]         = true;
};


function TrenchShovelImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armattack);
	%obj.schedule(200, playthread, 2, root);
}
function TrenchDirtImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, armattack);
	%obj.schedule(200, playthread, 2, root);
}
function TrenchShovelImage::onFire(%this,%obj,%slot)
{
	TakeChunk(%obj.client,1);
}
function TrenchDirtImage::onFire(%this,%obj,%slot)
{
	if(%obj.client.trenchDirt > 0)
		Parent::onFire(%this,%obj,%slot);
	ShootChunk(%obj.client);
}
function AdminShovelImage::onFire(%this,%obj,%slot)
{
	TakeChunk(%obj.client,1);
}
function AdminDirtImage::onFire(%this,%obj,%slot)
{
	if(%obj.client.trenchDirt > 0)
		Parent::onFire(%this,%obj,%slot);
	ShootChunk(%obj.client);
}