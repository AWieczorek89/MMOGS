using BackgroundManagement.Interfaces;
using BackgroundManagement.Measurement;
using BackgroundManagement.Models.ClientExchangeData;
using BackgroundManagement.Models.GameState;
using MMOC.BackgroundManagement;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace BackgroundManagement.GameHelpers.LocalCharacterBuilding
{
    public class StandardLocalCharacterBuilder : LocalCharacterBuilder
    {
        //key: model code; values: prefab source, appearance handler type name
        public static Dictionary<string, Tuple<string, Type>> ModelCodeDataDictionary { get; } = new Dictionary<string, Tuple<string, Type>>()
        {
            { "capsule", Tuple.Create("Prefabs/LocalPlace/CharacterAppearances/CapsuleCharAppearance", typeof(CapsuleCharAppearanceHandler)) },
            { "box", Tuple.Create("Prefabs/LocalPlace/CharacterAppearances/BoxCharAppearance", typeof(BoxCharAppearanceHandler)) }
        };

        private GameObject _localCharacterInstance;
        private LocalCharacterHandler _localCharacterHandler;
        private ISpecificSceneManager _sceneManager;
        private GameObject _charAppearanceInstance;
        private ICharacterAppearanceHandler _charAppearanceHandler;
        
        public StandardLocalCharacterBuilder(LocalCharacterDetails details, GameStateDetails gameStateDetails,  Camera camera)
            : base (details, gameStateDetails, camera)
        {
        }

        public override void InstantiateOnScene(ISpecificSceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            GameObject charPrefab = (GameObject)Resources.Load("Prefabs/LocalPlace/LocalCharacter", typeof(GameObject));
            _localCharacterInstance = _sceneManager.InstantiateExternally(charPrefab, null);
            _localCharacterInstance.transform.position = new Vector3(0.5f, 1f, 0.5f);
            _localCharacterHandler = _localCharacterInstance.GetComponent<LocalCharacterHandler>();
        }

        public override void SetCameraInstance()
        {
            _localCharacterHandler.CameraInstance = _camera;
        }

        public override void SetAppearance()
        {
            Tuple<string, Type> charAppDataTuple = null;

            if (!StandardLocalCharacterBuilder.ModelCodeDataDictionary.TryGetValue(_details.ModelCode, out charAppDataTuple))
            {
                Debug.Log($"Standard local character builder - cannot get appearance data by model code [{_details.ModelCode}]");
                return;
            }

            GameObject charAppearancePrefab = (GameObject)Resources.Load(charAppDataTuple.Item1, typeof(GameObject));
            GameObject appearanceBase = _localCharacterInstance.transform.Find("AppearanceBase").gameObject;
            _charAppearanceInstance = _sceneManager.InstantiateExternally(charAppearancePrefab, appearanceBase.transform);

            _charAppearanceHandler = (ICharacterAppearanceHandler)_charAppearanceInstance.GetComponent(charAppDataTuple.Item2);
        }

        public override void SetLocalCharacterHandler()
        {
            int mainCharId = _gameStateDetails.CharId;
            _localCharacterHandler.Set((mainCharId == _details.CharId), _details, _charAppearanceHandler);
        }

        public override void SetMovementAndPosition()
        {
            LocalCharacterHandler.MovementType movementType = LocalCharacterHandler.MovementType.Idle;
            if (_details.State.Equals("moving", GlobalData.InputDataStringComparison))
                movementType = LocalCharacterHandler.MovementType.Moving;

            int timeArrivalMs = 0;
            if (movementType == LocalCharacterHandler.MovementType.Moving)
            {
                DateTime movingStartTime = DateTime.Now;
                DateTime movingEndTime = DateTime.Now;

                if
                (
                    DateTime.TryParseExact(_details.MovingStartTime, GlobalData.MovementTimeExchangeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out movingStartTime) &&
                    DateTime.TryParseExact(_details.MovingEndTime, GlobalData.MovementTimeExchangeFormat, CultureInfo.InstalledUICulture, DateTimeStyles.None, out movingEndTime)
                )
                {
                    timeArrivalMs = Measure.GetTimeMsBetween(movingEndTime, movingStartTime);
                }
            }
            
            _localCharacterHandler.UpdateMovement
            (
                movementType,
                _details.DestinationLoc,
                _details.Angle,
                timeArrivalMs,
                false
            );
        }

        public override GameObject GetInstance()
        {
            return _localCharacterInstance;
        }
    }
}
