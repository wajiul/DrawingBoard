using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using SignalRSample.Models;
using SignalRSample.Repositories;

namespace DrawingBoard.Controllers
{


    public class BoardController : Controller
    {
        private readonly BoardRepository _boardRepository;
        private readonly UserRepository _userRepository;
        public BoardController(BoardRepository boardRepository, UserRepository userRepository)
        {
            _boardRepository = boardRepository;
            _userRepository = userRepository;
        }


        public IActionResult Display(BoardUser user)
        {
            var shareLink = $"https://localhost:7041/board/share/{user.BoardId}";
            ViewBag.shareLink = shareLink;
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var existing = await _userRepository.GetUserByEmailAsync(user.Email);

            if (existing == null)
            {
                var board = new Board
                {
                    Title = "Untitled",
                    Date = DateTime.Now,
                };

                user.Boards.Add(board);


                var newUser = await _userRepository.AddUserAsync(user);
                await _userRepository.SaveAsync();

                var newBoard = newUser.Boards.Take(1).FirstOrDefault();
                var boardUser = new BoardUser
                {
                    BoardId = newBoard.BoardId,
                    UserId = newUser.UserId,
                    FullName = user.FullName,
                    IsShared = false
                };
                return RedirectToAction("Display", boardUser);
            }

            return RedirectToAction("MyBoard", new { userId =  existing.UserId.ToString() });
        }


        public async Task<IActionResult> MyBoard(string userId)
        {
            var user = await _userRepository.GetUserAsync(new Guid(userId));
            return View(user); 
        }



        [HttpGet]
        public IActionResult Share(string Id)
        {

            var boardUser = new BoardUser
            {
                BoardId = new Guid(Id),
                IsShared = true
            };
            return View(boardUser);
        }

        [HttpPost]
        public IActionResult Share(BoardUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            return RedirectToAction("Display", user);
        }


        public IActionResult Library()
        {
            return View();
        }


        public async Task<IActionResult> AddNewBoard(Guid userId)
        {
            var user = await _userRepository.GetUserAsync(userId);

            var board = new Board
            {
                UserId = userId,
                Title = "Untitled",
                Date = DateTime.Now
            };

            await _boardRepository.AddAsync(board);
            await _boardRepository.SaveAsync();

            var boardUser = new BoardUser
            {
                BoardId = board.BoardId,
                FullName = user.FullName,
                UserId = user.UserId,
                IsShared = false
            };

            return RedirectToAction("Display", boardUser);
        }
        
        public async Task<IActionResult> GetAllUsersBoards()
        {
            var boars = await _boardRepository.GetAllUsersBoardsAsync();
            return Ok(boars);   
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAllBoards(string userId)
        {
            var boards = await _boardRepository.GetUserBoardsAsync(new Guid(userId)); 
            return Ok(boards);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBoard(Guid boardId)
        {
            await _boardRepository.DeleteBoardAsync(boardId);   
            return Ok(new { id = boardId});
        }

    }
}
