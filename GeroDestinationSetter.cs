using UnityEngine;
using System.Collections;

namespace Pathfinding {
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_destination_setter.php")]
	public class GeroDestinationSetter : VersionedMonoBehaviour {
		/// <summary>The object that the AI should move to</summary>
        
		[SerializeField]
        private Vector3 targetV3;
        
		[SerializeField]
		private Transform targetTransform;
		
		IAstarAI ai;

		void OnEnable () {
			ai = GetComponent<IAstarAI>();
			// Update the destination right before searching for a path as well.
			// This is enough in theory, but this script will also update the destination every
			// frame as the destination is used for debugging and may be used for other things by other
			// scripts as well. So it makes sense that it is up to date every frame.
			if (ai != null) ai.onSearchPath += Update;
		}

		void OnDisable () {
			if (ai != null) ai.onSearchPath -= Update;
		}

		/// <summary>Updates the AI's destination every frame</summary>
		void Update () {
            if (targetTransform != null)
            {
                if (ai != null) ai.destination = targetTransform.position;
            } else if(targetV3 == Vector3.zero)
            {
                if (ai != null) ai.destination = targetV3;
            }
		}

		public void SetDestination(Vector3 target) {
			targetV3 = target;
			targetTransform = null;
		}

		public void SetDestination(Transform target){
			targetTransform = target;
			targetV3 = Vector3.zero;
		}

		public void ClearDestination(){
			targetTransform = null;
			targetV3 = Vector3.zero;
		}
	}
}
