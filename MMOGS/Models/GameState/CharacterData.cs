using MMOGS.Measurement;
using MMOGS.Measurement.Units;
using MMOGS.Models.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MMOGS.Models.GameState
{
    public class CharacterData : IDisposable
    {
        public enum CharacterState
        {
            Idle,
            Moving,
            WorldMap
        }

        private readonly object _dataLock = new object();
        private static readonly int _movementCalcTickMs = 100; //movement calculation interval for each character
        
        #region Basic non-locked variables
        
        private CharacterState _state = CharacterState.Idle;
        private double _velocity = 0.00;
        private double _angle = 0.00;
        private int _wmId = -1;
        private int _parentObjectId = -1;
        private bool _isOnWorldMap = false;
        private int _hairstyleId = -1;
        private string _name = "";
        private string _modelCode = "";

        //world loc.
        private Point2<int> _currentWorldLoc;
        
        //local loc.
        private Point3<double> _startingLoc;
        private Point3<double> _destinationLoc;
        private Point3<double> _currentLoc;
        private DateTime _movingStartTime;
        private DateTime _movingEndTime;

        #endregion

        #region Thread-locked properties

        public int WmId
        {
            get { lock (_dataLock) { return _wmId; } }
            set { lock (_dataLock) { _wmId = value; } }
        }

        public int ParentObjectId
        {
            get { lock (_dataLock) { return _parentObjectId; } }
            set { lock (_dataLock) { _parentObjectId = value; } }
        }

        public bool IsOnWorldMap
        {
            get
            {
                lock (_dataLock)
                {
                    return _isOnWorldMap;
                }
            }
            set
            {
                lock (_dataLock)
                {
                    _state = (value == true ? CharacterState.WorldMap : CharacterState.Idle);
                    _isOnWorldMap = value;
                }
            }
        }

        public Point2<int> CurrentWorldLoc
        {
            get { lock (_dataLock) { return _currentWorldLoc.Copy(); } }
            private set { lock (_dataLock) { _currentWorldLoc = value; } }
        }

        public string ModelCode
        {
            get { lock (_dataLock) { return _modelCode; } }
            set { lock (_dataLock) { _modelCode = value; } }
        }

        public string Name
        {
            get { lock (_dataLock) { return _name; } }
            set { lock (_dataLock) { _name = value; }  }
        }

        public int HairstyleId
        {
            get { lock (_dataLock) { return _hairstyleId; } }
            set { lock (_dataLock) { _hairstyleId = value; } }
        }

        public DateTime MovingEndTime
        {
            get { lock (_dataLock) { return _movingEndTime; } }
            private set { lock (_dataLock) { _movingEndTime = value; } }
        }

        public DateTime MovingStartTime
        {
            get { lock (_dataLock) { return _movingStartTime; } }
            private set { lock (_dataLock) { _movingStartTime = value; } }
        }

        public Point3<double> CurrentLoc
        {
            get { lock (_dataLock) { return _currentLoc.Copy(); } }
            private set { lock (_dataLock) { _currentLoc = value; } }
        }

        public Point3<double> DestinationLoc
        {
            get { lock (_dataLock) { return _destinationLoc.Copy(); } }
            private set { lock (_dataLock) { _destinationLoc = value; } }
        }

        public Point3<double> StartingLoc
        {
            get { lock (_dataLock) { return _startingLoc.Copy(); } }
            private set { lock (_dataLock) { _startingLoc = value; } }
        }
        
        public CharacterState State
        {
            get { lock (_dataLock) { return _state; } }
            private set { lock (_dataLock) { _state = value; } }
        }

        public double Velocity
        {
            get { lock (_dataLock) { return _velocity; } }
            private set { lock (_dataLock) { _velocity = value; } }
        }

        public double Angle
        {
            get { lock (_dataLock) { return _angle; } }
            private set { lock (_dataLock) { _angle = value; } }
        }
        
        #endregion

        public int AccId { get; private set; } = -1;
        public int CharId { get; private set; } = -1;

        private DbCharactersData _dbData = null;
        private bool _movementCalculationInProgress = false;
        
        public CharacterData(DbCharactersData dbData)
        {
            _dbData = dbData ?? throw new Exception("DB data of new character cannot be NULL!");
            GameWorldData gameWorldData = GameWorldData.GetLastInstance();
            
            this.StartingLoc = new Point3<double>(Convert.ToDouble(_dbData.LocalPosX), Convert.ToDouble(_dbData.LocalPosY), Convert.ToDouble(_dbData.LocalPosZ));
            this.DestinationLoc = this.StartingLoc;
            this.CurrentLoc = this.StartingLoc;
            this.Angle = Convert.ToDouble(_dbData.LocalAngle);

            DateTime tNow = DateTime.Now;
            this.MovingStartTime = tNow;
            this.MovingEndTime = tNow;

            this.AccId = _dbData.AccId;
            this.CharId = _dbData.CharId;
            this.WmId = _dbData.WmId;
            this.ParentObjectId = _dbData.TerrainParentId;
            this.IsOnWorldMap = _dbData.IsOnWorldMap;
            this.CurrentWorldLoc = gameWorldData.GetWorldCoordsByWmId(_dbData.WmId);
            
            if (_dbData.IsOnWorldMap)
                this.State = CharacterState.WorldMap;

            this.ModelCode = _dbData.ModelCode;
            this.HairstyleId = _dbData.HairstyleId;
            this.Name = (_dbData.IsNpc ? _dbData.NpcAltName : _dbData.Name);

            CalculateMovementAsync();
        }

        public void Dispose()
        {
            _movementCalculationInProgress = false;
            _dbData = null;
        }
        
        public DbCharactersData GetDbData()
        {
            return _dbData.Copy();
        }

        public void MoveCharacterWorld(Point2<int> newLocation)
        {
            this.CurrentWorldLoc = newLocation;
            this.State = CharacterState.WorldMap;
        }

        public void MoveCharacterLocal(Point3<double> oldLocation, Point3<double> newLocation, int timeArrivalMs)
        {
            if (timeArrivalMs < 0) timeArrivalMs = 0;

            this.StartingLoc = oldLocation;
            this.CurrentLoc = this.StartingLoc;
            this.DestinationLoc = newLocation;
            
            this.MovingStartTime = DateTime.Now;
            this.MovingEndTime = this.MovingStartTime.AddMilliseconds(timeArrivalMs);

            this.State = CharacterState.Moving;
        }
        
        public void GetLocationLocal(out Point3<double> currentLoc)
        {
            currentLoc = this.CurrentLoc;
        }

        public void GetLocationLocal
        (
            out Point3<double> startingLoc, 
            out Point3<double> destinationLoc, 
            out Point3<double> currentLoc
        )
        {
            startingLoc = this.StartingLoc;
            destinationLoc = this.DestinationLoc;
            currentLoc = this.CurrentLoc;
        }

        public string GetCharacterName()
        {
            return _dbData.Name;
        }

        private async void CalculateMovementAsync()
        {
            _movementCalculationInProgress = true;
            
            int movementTotalMs = 0;
            int movementCurrentMs = 0;
            TimeSpan tSpanTotal;
            TimeSpan tSpanCurrent;

            DateTime tNow;
            double tParam = 1.00;
            double movementTotalDistance = 0.00;

            do
            {
                tNow = DateTime.Now;
                
                if (this.State == CharacterState.Moving)
                {
                    tSpanTotal = this.MovingEndTime - this.MovingStartTime;
                    tSpanCurrent = tNow - this.MovingStartTime;
                    movementTotalMs = (int)tSpanTotal.TotalMilliseconds;
                    movementCurrentMs = (int)tSpanCurrent.TotalMilliseconds;

                    if ((movementCurrentMs >= movementTotalMs) || movementTotalMs <= 0)
                    {
                        this.State = CharacterState.Idle;
                        this.Velocity = 0.00;

                        this.StartingLoc = this.DestinationLoc;
                        this.CurrentLoc = this.DestinationLoc;
                        this.MovingStartTime = tNow;
                        this.MovingEndTime = tNow;
                        
                        continue;
                    }
                    
                    tParam = Convert.ToDouble(movementCurrentMs) / Convert.ToDouble(movementTotalMs);
                    movementTotalDistance = Measure.GetDistanceBetweenPoints(this.StartingLoc, this.DestinationLoc);
                    
                    SetCurrentLocReference
                    (
                        Measure.Lerp(this.StartingLoc.X, this.DestinationLoc.X, tParam),
                        Measure.Lerp(this.StartingLoc.Y, this.DestinationLoc.Y, tParam),
                        Measure.Lerp(this.StartingLoc.Z, this.DestinationLoc.Z, tParam)
                    );

                    //Console.WriteLine($"current {this.CurrentLoc.GetCoordsString()}");
                    
                    this.Velocity = movementTotalDistance / movementTotalMs; //v = s / t
                    this.Angle = Measure.GetAngleBetweenVectors
                    (
                        new Point2<double>(1, 0), //X axis
                        new Point2<double>(this.DestinationLoc.X - this.StartingLoc.X, this.DestinationLoc.Y - this.StartingLoc.Y)
                    );
                }
                else
                {
                    this.Velocity = 0.00;
                }

                await Task.Factory.StartNew(() => Thread.Sleep(CharacterData._movementCalcTickMs));
            }
            while (_movementCalculationInProgress);
        }

        private void SetCurrentLocReference(double x, double y, double z)
        {
            lock (_dataLock)
            {
                _currentLoc.X = x;
                _currentLoc.Y = y;
                _currentLoc.Z = z;
            }
        }
    }
}
