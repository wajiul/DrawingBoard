using DrawingBoard.Models;
using Microsoft.EntityFrameworkCore;
using SignalRSample.Data;
using SignalRSample.Models;

namespace SignalRSample.Repositories
{
    public class BoardRepository
    {
        private readonly ApplicationDbContext _context;

        public BoardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Board board)
        {
            await _context.Boards.AddAsync(board);
        }

        public async Task AddCanvasObject(CanvasObject canvasObject)
        {
            await _context.CanvasObjects.AddAsync(canvasObject);    
        }

        public async Task<Board> GetBoardAsync(Guid boardId)
        {
            var board = await _context.Boards
                    .Include(c => c.CanvasObjects)
                    .FirstOrDefaultAsync(u => u.BoardId == boardId);
            return board;
        }

        public async Task<CanvasDto> GetCanvasAsync(Guid boardId)
        {
            var canvasObjects = await _context.CanvasObjects.Where(b => b.BoardId == boardId).ToListAsync();

            var canvas = new CanvasDto();
            canvas.BoardId = boardId;
            foreach (var canvasObject in canvasObjects)
            {
                canvas.CanvasObjects.Add(canvasObject);
            }
            return canvas;
        }

        public async Task<BoardUser?> GetBoardWithUserAsync(string boardId)
        {
            return await _context.Boards
                .Where(b => b.BoardId == new Guid(boardId))
                .Select(b => new BoardUser
                {
                    BoardId = b.BoardId.ToString(),
                    BoardTitle = b.Title,
                    UserId = b.UserId.ToString(),
                    UserName = b.User.FullName,

                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<BoardModel>> GetBoardsDetailsWithoutCanvasAsync(Guid userId)
        {
           return await _context.Boards
                .Where(b => b.UserId == userId)
                .Select(b => new BoardModel
                {
                    BoardId = b.BoardId.ToString(),
                    Title= b.Title,
                    UserId = b.UserId.ToString(),
                    Date = b.Date
                }).ToListAsync();
        }

        public async Task<IEnumerable<BoardWithAuthorModel>> GetAllUsersBoardDetailsWithoutCanvasAsync()
        {
            return await _context.Boards
                .Select(b => new BoardWithAuthorModel
                {
                    BoardId = b.BoardId.ToString(),
                    Title = b.Title,
                    UserId = b.UserId.ToString(),
                    Date = b.Date,
                    Author = b.User.FullName,
                    Email = b.User.Email
                }).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersBoardsAsync()
        {
            return await _context.Users
                .Include(b => b.Boards)
                    .ThenInclude(c => c.CanvasObjects)
                .ToListAsync();
        }

        public async Task<User> GetUserBoardsAsync(Guid userId)
        {
            var userBoard = await _context.Users
                .Include(b => b.Boards)
                    .ThenInclude(c => c.CanvasObjects)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            return userBoard;
        }

        public async Task<IEnumerable<BoardCanvasModel>> GetAllBoardCanvasOfUserAsync(Guid userId)
        {
           return await _context.Boards
                .Where(b => b.UserId == userId)
                .Select(b => new BoardCanvasModel
                {
                    BoardId= b.BoardId.ToString(),
                    CanvasObjects = b.CanvasObjects

                }).ToListAsync();
        }

        public async Task<IEnumerable<BoardCanvasModel>> GetBoardCanvasOfAllUserAsync()
        {
            return await _context.Boards

                 .Select(b => new BoardCanvasModel
                 {
                     BoardId = b.BoardId.ToString(),
                     CanvasObjects = b.CanvasObjects

                 }).ToListAsync();
        }


        public async Task<User> GetUserBoardAsync(Guid userId)
        {
            var board = await _context.Users
                .Include(u => u.Boards)
                .FirstOrDefaultAsync (u => u.UserId == userId);

            return board;
        }

        public async Task<CanvasObject> GetCanvasObjectAsync(Guid objectId)
        {
            return await _context.CanvasObjects.FindAsync(objectId);
        }

        public void UpdateCanvasObject(CanvasObject canvasObject)
        {
            try
            {
                 _context.CanvasObjects.Update(canvasObject);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public async Task DeleteCanvasObjectAsync(Guid canvasId)
        {
            var canvasObject = await _context.CanvasObjects.FindAsync(canvasId);
            if (canvasObject != null)
            {
                _context.CanvasObjects.Remove(canvasObject);
            }
        }

        public async Task UpdateBoardTitleAsync(Guid boardId, string title)
        {
            var board = await _context.Boards.FindAsync(boardId);
            if (board != null)
            {
                board.Title = title;
                _context.Boards.Update(board);
            }
        }

        public async Task DeleteBoardAsync(Guid boardId)
        {
            var board = await _context.Boards.FindAsync(boardId);
            if(board != null)
            {
                _context.Boards.Remove(board);
            }
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
