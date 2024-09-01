using Microsoft.AspNetCore.SignalR;
using SignalRSample.Models;
using SignalRSample.Repositories;
using System.Text.Json;

namespace SignalRSample.Hubs
{

    public class BoardHub : Hub
    {
        private readonly BoardRepository _boardRepository;
        private static readonly Dictionary<string, List<string>> GroupUsers = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, Stack<string>> UndoStack = new Dictionary<string, Stack<string>>();
        private static readonly Dictionary<string, Stack<string>> RedoStack = new Dictionary<string, Stack<string>>();
        private static readonly Dictionary<string, string> ConnectionToUserMap = new Dictionary<string, string>();

        public BoardHub(BoardRepository boardRepository)
        {
            _boardRepository = boardRepository;
        }


       
        private async Task<string> GetCanvasJsonString(Guid boardId)
        {
            var canvas = await _boardRepository.GetCanvasAsync(boardId);
            string canvasString = JsonSerializer.Serialize(canvas);
            return canvasString;
        }
        public async Task AddOrUpdateObject(CanvasObject canvasObject)
        {
            string groupId = canvasObject.BoardId.ToString();
            var existingObject = await _boardRepository.GetCanvasObjectAsync(canvasObject.ObjectId);

            if (existingObject != null)
            {
                existingObject.ObjectData = canvasObject.ObjectData;
            }
            else
            {
                await _boardRepository.AddCanvasObject(canvasObject);
            }
            await _boardRepository.SaveAsync();

            

            var canvas = await GetCanvasJsonString(canvasObject.BoardId);

            UndoStack[groupId].Push(canvas);
            await Clients.Group(groupId).SendAsync("ReceiveCanvasObject", canvasObject);
        }

        public async Task DeleteObject(string boardId, string objectId)
        {
            await _boardRepository.DeleteCanvasObjectAsync(new Guid(objectId));
            await _boardRepository.SaveAsync();

            UndoStack[boardId].Push(await GetCanvasJsonString(new Guid(boardId)));

            await Clients.Group(boardId).SendAsync("ReceiveDeletedCanvasObjectId", objectId);
        }

        public async Task<Board> GetBoard(string boardId)
        {
            var board = await _boardRepository.GetBoardAsync(new Guid(boardId));

            if (!UndoStack.ContainsKey(boardId))
            {
                UndoStack[boardId] = new Stack<string>();
                RedoStack[boardId] = new Stack<string>();
                UndoStack[boardId].Push(await GetCanvasJsonString(new Guid(boardId)));
            }

            return board;
        }

        
        public async Task JoinBoard(string boardId, string userName)
        {

            string connectionId = Context.ConnectionId;

            await Groups.AddToGroupAsync(connectionId, boardId);

            lock (GroupUsers)
            {
                if (!GroupUsers.ContainsKey(boardId))
                {
                    GroupUsers[boardId] = new List<string>();
                }

                if (!GroupUsers[boardId].Contains(connectionId))
                {
                    GroupUsers[boardId].Add(connectionId);
                }
            }

            ConnectionToUserMap[connectionId] = userName;

            await Clients.OthersInGroup(boardId).SendAsync("UserJoined", userName);
        }

        
        public async Task LeaveBoard(string boardId, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, boardId);

            lock (GroupUsers)
            {
                if (GroupUsers.ContainsKey(boardId))
                {
                    GroupUsers[boardId].Remove(userName);

                    if (GroupUsers[boardId].Count == 0)
                    {
                        GroupUsers.Remove(boardId);
                    }
                }
            }

            await Clients.Group(boardId).SendAsync("UserLeft", userName);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = Context.ConnectionId;
            string userName = ConnectionToUserMap[connectionId];
            
            var boards = GroupUsers.Where(b => b.Value.Contains(connectionId)).Select(b => b.Key).ToList();

            foreach (var boardId in boards)
            {
                GroupUsers[boardId].Remove(connectionId);

                if (GroupUsers[boardId].Count == 0)
                {
                    GroupUsers.Remove(boardId);
                }

                await Clients.Group(boardId).SendAsync("UserLeft", userName);
                ConnectionToUserMap.Remove(connectionId);

                await Groups.RemoveFromGroupAsync(connectionId, boardId);
            }

            await base.OnDisconnectedAsync(exception);

        }


        
        public async Task UsersInGroup(string boardId)
{
            var users = new List<string>();
            lock (GroupUsers)
            {
                if (GroupUsers.ContainsKey(boardId))
                {
                    users = GroupUsers[boardId].ToList();

                }
            }

            var userNames = new List<string>();
            foreach (var user in users)
            {
                userNames.Add(ConnectionToUserMap[user]);
            }
            await Clients.Group(boardId).SendAsync("ReceiveUsersInGroup", userNames);
        }

        public async Task Undo(Guid boardId)
        {

            var groupId = boardId.ToString();
            string state = "";
            if (UndoStack.ContainsKey(groupId) && UndoStack[groupId].Count > 0)
            {
                var top = UndoStack[groupId].Pop();
                RedoStack[groupId].Push(top);
                if (UndoStack[groupId].Count > 0)
                {
                    state = UndoStack[groupId].Peek();
                }
            }

            CanvasDto deserializedState = JsonSerializer.Deserialize<CanvasDto>(state);

            await Clients.Group(groupId).SendAsync("ReceiveUndoRedoState", deserializedState);
        }
        public async Task Redo(Guid boardId)
        {
            var groupId = boardId.ToString();
            var state = "";
            if (RedoStack[groupId].Count > 0)
            {
                state = RedoStack[groupId].Pop();
                UndoStack[groupId].Push(state); 
            }

            CanvasDto deserializedState = JsonSerializer.Deserialize<CanvasDto>(state);
            await Clients.Group(groupId).SendAsync("ReceiveUndoRedoState", deserializedState);
        }


        public async Task UpdateTitle(string boardId, string title)
        {
            var groupId = boardId.ToString();
            await _boardRepository.UpdateBoardTitleAsync(new Guid(boardId), title);
            await _boardRepository.SaveAsync();
            await Clients.Group(groupId).SendAsync("ReceiveTitle", title);
        }
    }

}

