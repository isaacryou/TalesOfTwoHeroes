using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TT.Player;
using PathCreation;

namespace TT.Player
{
    public class TT_Player_PathFollower : MonoBehaviour
    {
        public PathCreator pathToFollow;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed;
        private float distanceTravelled = 0;

        void Start()
        {
            StartMovingTowardPoint(0, false);
        }

        void Update()
        {
        }

        //Start coroutine to move towrard point
        public void StartMovingTowardPoint(float _distanceToTravel, bool _firstCallIndicator)
        {
            StartCoroutine(MoveTowardsPoint(_distanceToTravel, _firstCallIndicator));
        }

        //Move toward point frame by frame
        IEnumerator MoveTowardsPoint(float _distanceToTravel, bool _firstCallIndicator)
        {
            distanceTravelled += speed * Time.deltaTime;

            bool callRecursion = true;

            if (_distanceToTravel <= distanceTravelled)
            {
                distanceTravelled = _distanceToTravel;

                callRecursion = false;
            }

            transform.position = pathToFollow.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);

            yield return new WaitForEndOfFrame();

            if (callRecursion)
            {
                yield return StartCoroutine(MoveTowardsPoint(_distanceToTravel, false));
            }

            if (_firstCallIndicator)
            {
                TT_Player_Player playerScript = GetComponent<TT_Player_Player>();

                playerScript.ChangeFromBoardView();
            }
        }
    }
}

