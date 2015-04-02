using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

//using System.Collections;

public class AnimationHelper : EditorWindow {

	public GameObject target;

	public AnimationClip idleAnim;
	public AnimationClip runAnim;
	public AnimationClip changeRunDirectionAnim;
	public AnimationClip jumpAnim;

	public AnimationClip headJumpedAnim;
	public AnimationClip dieAnim;
	public AnimationClip gameOverAnim;
	public AnimationClip spawnAnim;
	public AnimationClip spawnProtectionAnim;
	public AnimationClip rageAnim;

	public SmwCharacter smwCharacter;

	[MenuItem ("Window/Animation Helper")]
	static void OpenWindow ()
	{
		// Get existing open window or if none, make a new one:
		GetWindow<AnimationHelper> ();
	}


	void OnGUI () {
		target = EditorGUILayout.ObjectField("Target Object", target, typeof(GameObject), true) as GameObject;

		smwCharacter = EditorGUILayout.ObjectField("SMW Character SO", smwCharacter, typeof(SmwCharacter), false) as SmwCharacter;

//		idleAnim = EditorGUILayout.ObjectField("Idle", idleAnim, typeof(AnimationClip), false) as AnimationClip;
//		runAnim = EditorGUILayout.ObjectField("Run", runAnim, typeof(AnimationClip), false) as AnimationClip;
//		jumpAnim = EditorGUILayout.ObjectField("Jump", jumpAnim, typeof(AnimationClip), false) as AnimationClip;
//		headJumpedAnim = EditorGUILayout.ObjectField("HeadJumped", headJumpedAnim, typeof(AnimationClip), false) as AnimationClip;
//		dieAnim = EditorGUILayout.ObjectField("Die", dieAnim, typeof(AnimationClip), false) as AnimationClip;


		if(GUILayout.Button("Create"))
		{
			if(target == null)
			{
				Debug.LogError ("No target for animator controller set.");
				return;
			}

			Create ();
		}
	}

	void Create ()
	{
		UnityEditor.Animations.AnimatorController controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/" + target.name + " scripted Animator.controller");

		// Add parameters
		controller.AddParameter(HashID.p_hSpeed, AnimatorControllerParameterType.Float);
		controller.AddParameter(HashID.p_vSpeed, AnimatorControllerParameterType.Float);
		controller.AddParameter(HashID.p_grounded, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_walled, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_changeRunDirectionTrigger, AnimatorControllerParameterType.Trigger);

		controller.AddParameter(HashID.p_hitTrigger, AnimatorControllerParameterType.Trigger);
		controller.AddParameter(HashID.p_nextStateTrigger, AnimatorControllerParameterType.Trigger);
		controller.AddParameter(HashID.p_hitted, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_gameOver, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_headJumped, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_dead, AnimatorControllerParameterType.Bool);

		controller.AddParameter(HashID.p_spawn, AnimatorControllerParameterType.Bool);
		controller.AddParameter(HashID.p_spawnProtection, AnimatorControllerParameterType.Bool);

		controller.AddParameter(HashID.p_rageTrigger, AnimatorControllerParameterType.Trigger);
		controller.AddParameter(HashID.p_rageModus, AnimatorControllerParameterType.Bool);
		
		// Add states
		AnimatorState idleState = controller.layers[0].stateMachine.AddState(HashID.s_Idle);
		idleState.motion = idleAnim;

		AnimatorState jumpState = controller.layers[0].stateMachine.AddState(HashID.s_JumpAndFall);
		jumpState.motion = jumpAnim;

		AnimatorState runState = controller.layers[0].stateMachine.AddState(HashID.s_Run);
		runState.motion = runAnim;

		AnimatorState changeRunDirectionState = controller.layers[0].stateMachine.AddState(HashID.s_ChangeRunDirection);
		changeRunDirectionState.motion = changeRunDirectionAnim;

		AnimatorState hittedState = controller.layers[0].stateMachine.AddState(HashID.s_Hitted);
//		hittedState.motion = idleAnim;

		AnimatorState headJumpedState = controller.layers[0].stateMachine.AddState(HashID.s_HeadJumped);
		headJumpedState.motion = headJumpedAnim;

		AnimatorState gameOverState = controller.layers[0].stateMachine.AddState(HashID.s_GameOver);
		gameOverState.motion = headJumpedAnim;

		AnimatorState deadState = controller.layers[0].stateMachine.AddState(HashID.s_Dead);
		deadState.motion = headJumpedAnim;

		AnimatorState spawnState = controller.layers[0].stateMachine.AddState(HashID.s_Generic_Spawn);
		spawnState.motion = headJumpedAnim;

		AnimatorState spawnProtectionState = controller.layers[0].stateMachine.AddState(HashID.s_Generic_SpawnProtection);
		spawnProtectionState.motion = headJumpedAnim;
		
		// State Transition Setup



		// AnimationClip

		AnimationClip animClip = new AnimationClip();

#if !UNITY_5
		// Setting it as generic allows you to use the animation clip in the animation controller
		AnimationUtility.SetAnimationType(animClip, ModelImporterAnimationType.Generic);
#endif

		animClip.name = "dynamic_Idle";
		CreateAnimationClip(animClip, smwCharacter.charIdleSprites, 1, singleSpriteTime);

		// for correct transform editing
		animClip.EnsureQuaternionContinuity();
		idleState.motion = animClip;
	}

	float singleSpriteTime = 0.0833f;

	void CreateAnimationClip(AnimationClip animClip, Sprite[] sprites, int animationLength, float singleSpriteTime)
	{
		// First you need to create e Editor Curve Binding
		EditorCurveBinding curveBinding = new EditorCurveBinding();
		
		// I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
		curveBinding.type = typeof(SpriteRenderer);
		// Regular path to the gameobject that will be changed (empty string means root)
		curveBinding.path = "";
		// This is the property name to change the sprite of a sprite renderer
		curveBinding.propertyName = "m_Sprite";
		
		// An array to hold the object keyframes
		ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[animationLength];
		
		for (int i = 0; i < sprites.Length; i++)
		{
			keyFrames[i] = new ObjectReferenceKeyframe();
			// set the time
			keyFrames[i].time = singleSpriteTime;
			// set reference for the sprite you want
			keyFrames[i].value = sprites[i];
			
		}
		
		AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames);
	}

//	int timeForKey(int i)
//	{
//		// Here you put the code that will return the time
//		// for the ith keyframe
//		
//		// Example: the time of each frame will be its index
//		return i;
//	}
//	
//	Sprite spriteForKey(int i)
//	{
//		// Here you put the code that will return the sprite that you want
//		// for the ith keyframe
//		
//		// Example: assuming you have a sprite array "_sprites" populated with all the sprites for each keyframe
//		// in order
//		return _sprites[i];
//	}
}
