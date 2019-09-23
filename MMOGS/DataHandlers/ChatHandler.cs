using MMOGS.DataHandlers.CommandBuilding;
using MMOGS.DataHandlers.CommandHandling;
using MMOGS.Interfaces;
using MMOGS.Measurement;
using MMOGS.Models.ClientExchangeData;
using MMOGS.Models.GameState;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MMOGS.DataHandlers
{
    public class ChatHandler : IDisposable
    {
        private ILogger _logger;
        private ICharacterInfo _characterInfo;
        private PlayerHandler _playerHandler;

        private readonly object _chatMsgLock = new object();
        private static readonly int _timeIntervalMs = 500;
        private static double _globalChatMaxDistance = 25;

        private bool _handlingInProgress = true;
        private List<Tuple<int, ChatMessageDetails>> _charIdMsgList = new List<Tuple<int, ChatMessageDetails>>();

        public ChatHandler(ILogger logger, ICharacterInfo characterInfo, PlayerHandler playerHandler)
        {
            _logger = logger ?? throw new Exception("Chat handler - logger cannot be NULL!");
            _characterInfo = characterInfo ?? throw new Exception("Chat handler - character info is NULL!");
            _playerHandler = playerHandler ?? throw new Exception("Chat handler - player handler is NULL!");

            HandleMessagesAsync();
        }

        private async void HandleMessagesAsync()
        {
            _logger.UpdateLog("Chat handler - handling started");

            int charId;
            ChatMessageDetails details;
            List<Tuple<int, ChatMessageDetails>> tempMsgList = new List<Tuple<int, ChatMessageDetails>>();
            PlayerDetails senderPlayer;
            CharacterData senderCharacter;
            PlayerDetails receiverPlayer;
            CharacterData receiverCharacter;
            List<CharacterData> receiverCharacterList = new List<CharacterData>();
            double distance = 0;

            do
            {
                await Task.Factory.StartNew(() => Thread.Sleep(_timeIntervalMs));

                if (!_handlingInProgress)
                    break;

                //TEMP LIST CREATION
                lock (_chatMsgLock)
                {
                    if (_charIdMsgList.Count > 0)
                    {
                        foreach (Tuple<int, ChatMessageDetails> msgData in _charIdMsgList)
                        {
                            charId = msgData.Item1;
                            if (charId < 0)
                                continue;
                            
                            tempMsgList.Add(Tuple.Create(charId, (ChatMessageDetails)msgData.Item2.Clone()));
                        }
                        
                        _charIdMsgList.Clear();
                    }
                }

                //TEMP LIST HANDLING
                
                foreach (Tuple<int, ChatMessageDetails> msgData in tempMsgList)
                {
                    charId = msgData.Item1;
                    details = msgData.Item2;

                    senderPlayer = await Task.Factory.StartNew(() =>  _playerHandler.GetPlayerByCurrentCharId(charId));
                    if (senderPlayer == null)
                    {
                        _logger.UpdateLog($"Chat handling - cannot find sender player by char. ID [{charId}]");
                        continue;
                    }

                    senderCharacter = await _characterInfo.GetCharacterByIdTaskStart(charId);
                    if (senderCharacter == null)
                    {
                        _logger.UpdateLog($"Chat handling - cannot find sender character by char. ID [{charId}]");
                        continue;
                    }

                    if (details.IsPrivate)
                    {
                        #region Private message
                        //PRIVATE MESSAGE

                        receiverCharacter = await _characterInfo.GetCharacterByNameTaskStart(details.To);

                        if (receiverCharacter == null)
                        {
                            CommandHandler.Send
                            (
                                new ChatMessageCmdBuilder
                                (
                                    new ChatMessageDetails()
                                    {
                                        IsPrivate = true,
                                        From = "SERVER",
                                        To = senderCharacter.Name,
                                        Message = $"Character [{details.To}] doesn't exist!"
                                    }
                                ),
                                senderPlayer
                            );
                            continue;
                        }

                        if (receiverCharacter.GetDbData().IsNpc)
                        {
                            CommandHandler.Send
                            (
                                new ChatMessageCmdBuilder
                                (
                                    new ChatMessageDetails()
                                    {
                                        IsPrivate = true,
                                        From = "SERVER",
                                        To = senderCharacter.Name,
                                        Message = $"Cannot send message to NPC [{details.To}]"
                                    }
                                ), 
                                senderPlayer
                            );
                            continue;
                        }
                        
                        receiverPlayer = _playerHandler.GetPlayerByCurrentCharId(receiverCharacter.CharId);
                        if (receiverPlayer == null)
                        {
                            CommandHandler.Send
                            (
                                new ChatMessageCmdBuilder
                                (
                                    new ChatMessageDetails()
                                    {
                                        IsPrivate = true,
                                        From = "SERVER",
                                        To = senderCharacter.Name,
                                        Message = $"Character's [{details.To}] player is not logged in!"
                                    }
                                ),
                                senderPlayer
                            );
                            continue;
                        }

                        //============== success

                        CommandHandler.Send //to receiver
                        (
                            new ChatMessageCmdBuilder
                            (
                                new ChatMessageDetails()
                                {
                                    IsPrivate = true,
                                    From = senderCharacter.Name,
                                    To = receiverCharacter.Name,
                                    Message = details.Message
                                }
                            ),
                            receiverPlayer
                        );

                        CommandHandler.Send //to sender
                        (
                            new ChatMessageCmdBuilder
                            (
                                new ChatMessageDetails()
                                {
                                    IsPrivate = true,
                                    From = senderCharacter.Name,
                                    To = receiverCharacter.Name,
                                    Message = details.Message
                                }
                            ),
                            senderPlayer
                        );

                        #endregion
                    }
                    else
                    {
                        #region Public message
                        //PUBLIC MESSAGE
                        if (senderCharacter.IsOnWorldMap)
                        {
                            CommandHandler.Send(new InfoCmdBuilder("Cannot send public message on world map!"), senderPlayer);
                            continue;
                        }
                        
                        await Task.Factory.StartNew
                        (
                            () => 
                            {
                                receiverCharacterList = _characterInfo.GetCharactersByWorldLocation(senderCharacter.WmId, senderCharacter.IsOnWorldMap, senderCharacter.ParentObjectId);
                                foreach (CharacterData receiverCharData in receiverCharacterList)
                                {
                                    distance = Measure.GetDistanceBetweenPoints(senderCharacter.CurrentLoc, receiverCharData.CurrentLoc);
                                    if (distance > _globalChatMaxDistance)
                                        continue;

                                    receiverPlayer = _playerHandler.GetPlayerByCurrentCharId(receiverCharData.CharId);
                                    if (receiverPlayer == null)
                                        continue;

                                    CommandHandler.Send
                                    (
                                        new ChatMessageCmdBuilder
                                        (
                                            new ChatMessageDetails()
                                            {
                                                IsPrivate = false,
                                                From = senderCharacter.Name,
                                                To = String.Empty,
                                                Message = details.Message
                                            }
                                        ),
                                        receiverPlayer
                                    );
                                }
                            }
                        );

                        receiverCharacterList.Clear();

                        #endregion
                    }
                }
                  
                tempMsgList.Clear();
            }
            while (_handlingInProgress);
            
            _logger.UpdateLog("Chat handler - handling stopped!");
        }

        public void AddMessage(int charId, ChatMessageDetails msgDetails)
        {
            if (charId < 0 || msgDetails == null)
                return;

            lock (_chatMsgLock)
            {
                Tuple<int, ChatMessageDetails> newData = Tuple.Create(charId, msgDetails);
                _charIdMsgList.Add(newData);
            }
        }

        public void Dispose()
        {
            _handlingInProgress = false;

            lock (_chatMsgLock)
            {
                _charIdMsgList.Clear();
            }
        }
        
    }
}
