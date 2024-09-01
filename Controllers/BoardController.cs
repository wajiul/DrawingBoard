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
        private readonly IConfiguration _configuration;

        public BoardController(BoardRepository boardRepository, UserRepository userRepository, IConfiguration configuration)
        {
            _boardRepository = boardRepository;
            _userRepository = userRepository;
            _configuration = configuration;
        }


        public async Task<IActionResult> Display(string boardId, string? userId = null, string? userName = null)
        {
            if(string.IsNullOrEmpty(boardId))
            {
                return NotFound();
            } 
            var boardUser = await _boardRepository.GetBoardWithUserAsync(boardId);
            var rootLink = _configuration["SiteUrl"];
            var shareLink = $"{rootLink}/board/share/{boardId}";
            ViewBag.ShareLink = shareLink;
            ViewBag.UserId = userId;
            ViewBag.UserName = userName;
            return View(boardUser);
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
                    Date = DateTime.UtcNow
                };

                user.Boards.Add(board);


                var newUser = await _userRepository.AddUserAsync(user);
                await _userRepository.SaveAsync();

                var newBoard = newUser.Boards.Take(1).FirstOrDefault();

                return RedirectToAction("Display", new { boardId = newBoard.BoardId.ToString(), userId = newUser.UserId.ToString() });
            }

            else
            {
                await _userRepository.UpdateNameAsync(existing.UserId, user.FullName);
                await _userRepository.SaveAsync();
            }

            return RedirectToAction("MyBoard", new { userId =  existing.UserId.ToString() });
        }


        public async Task<IActionResult> Delete(string boardId, string userId)
        {
            var boardExist = await _boardRepository.DoesBoardExist(new Guid(boardId),new Guid(userId));
            if(!boardExist)
            {
                return NotFound();
            }
            var board = await _boardRepository.GetBoardDetailsWithoutCanvasAsync(new Guid(boardId));
            ViewData["UserId"] = userId;
            return View(board);
        }


        public async Task<IActionResult> DeleteConfirmed(string boardId, string userId)
        {
            if(string.IsNullOrEmpty(boardId))
            {
                return NotFound();
            }
            await _boardRepository.DeleteBoardAsync(new Guid(boardId));
            await _boardRepository.SaveAsync();
            return RedirectToAction("MyBoard", new { userId });
        }

        public async Task<IActionResult> MyBoard(string userId)
        {
            var boards = await _boardRepository.GetBoardsDetailsWithoutCanvasAsync(new Guid(userId));
            var user = await _userRepository.GetUserAsync(new Guid(userId));
            ViewData["UserId"] = userId;
            ViewData["UserName"] = user.FullName;
            return View(boards); 
        }

        [HttpGet]
        public IActionResult Share(string Id)
        {
            var boardUser = new BoardUser
            {
                BoardId = Id
            };
            return View(boardUser);
        }

        [HttpPost]
        public IActionResult Share(BoardUser user)
        {
            if (!ModelState.IsValid)
                return View(user);

            return RedirectToAction("Display", new {boardId = user.BoardId, userName = user.UserName});
        }


        public async Task<IActionResult> Library(string userId)
        {
            if(string.IsNullOrEmpty(userId)) 
            {
                return NotFound();
            }
            var boards = await _boardRepository.GetAllUsersBoardDetailsWithoutCanvasAsync();
            var user = await _userRepository.GetUserAsync(new Guid(userId));
            ViewData["UserId"] = userId;
            ViewData["UserName"] = user.FullName;
            return View(boards);
        }


        public async Task<IActionResult> NewBoard(string userId)
        {
            var user = await _userRepository.GetUserAsync(new Guid(userId));

            var board = new Board
            {
                UserId = new Guid(userId),
                Title = "Untitled",
                Date = DateTime.UtcNow
            };

            await _boardRepository.AddAsync(board);
            await _boardRepository.SaveAsync();


            return RedirectToAction("Display", new { boardId = board.BoardId.ToString(), userId = board.UserId.ToString() });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBoardCanvas()
        {
            var allCanvas = await _boardRepository.GetBoardCanvasOfAllUserAsync();
            return Ok(allCanvas);   
        }

        [HttpGet]
        public async Task<IActionResult> GetBoardCanvas(string boardId)
        {
            if(string.IsNullOrEmpty(boardId))
            {
                return NotFound();
            }
            var canvas = await _boardRepository.GetBoardCanvasAsync(new Guid(boardId));
            return Ok(canvas);
        } 

        [HttpGet]
        public async Task<IActionResult> GetUserAllBoards(string userId)
        {
            //var boards = await _boardRepository.GetUserBoardsAsync(new Guid(userId)); 
            var boards = await _boardRepository.GetAllBoardCanvasOfUserAsync(new Guid(userId));
            return Ok(boards);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBoard(Guid boardId)
        {
            await _boardRepository.DeleteBoardAsync(boardId);   
            return Ok(new { id = boardId});
        }


        [HttpPost]
        public async Task<IActionResult> UpdateBoardTitle(string boardId, string title)
            {
            if(string.IsNullOrEmpty(title))
            {
                return BadRequest();
            }
            await _boardRepository.UpdateBoardTitleAsync(new Guid(boardId), title);
            await _boardRepository.SaveAsync();
            return Ok(title);
        }
    }
}
