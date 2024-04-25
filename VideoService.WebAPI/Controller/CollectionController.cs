using Amazon.Runtime.Internal;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VideoService.Domain.Entities;
using VideoService.Infrastructure;
using VideoService.Infrastructure.Request;

namespace VideoService.WebAPI.Controller
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CollectionController : ControllerBase
    {
        private readonly VideoDbContext _db;

        private readonly IValidator<AddConnectionRequest> validator;
        public CollectionController(VideoDbContext videoDbContext, IValidator<AddConnectionRequest> validator)
        {
            this._db = videoDbContext;
            this.validator = validator;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Dictionary<string, string> dic)
        {
            string name = dic["name"];
            if (string.IsNullOrEmpty(name) || name.Length > 15)
                return BadRequest(new { result = false, mesg = "确保文件名不为空,名称需要低于15" });
            long us_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var exit = await _db.Collections
            .Include(x => x.Item).Where(x => x.CreateUserId == us_id && x.Name == name).CountAsync();

            if (exit != 0)
            {
                return BadRequest(new { result = false, mesg = "文件夹已存在" });
            }
            var coll = new Collection(name, us_id);
            await _db.Collections.AddAsync(coll);
            await _db.SaveChangesAsync();
            return Ok(new { result = true, data = coll });
        }


        [HttpPut("{videId}")]
        public async Task<IActionResult> Remove(int videId)
        {
            long us_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var video = await _db.Video.SingleOrDefaultAsync(x => x.Id == videId);
            var items = await _db.Collections.Include(x => x.Item).Where(x => x.CreateUserId == us_id).Where(x => x.Item.Any(v => v.VideoId == videId)).SingleAsync();
            _db.Video.Update(video.RemoveCollect());
            this._db.CollectionItems.Remove(items.Item[0]);
            _db.SaveChanges();
            return Ok(new { result = true,data = items });
        }

        [HttpPut]
        public async Task<IActionResult> Rename(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName) || oldName.Length > 8 && string.IsNullOrEmpty(newName) || newName.Length > 8)
            {
                return Ok(new { result = false, mesg = "名称小于8" });
            }
            if (oldName == newName) return Ok(new { result = true });

            long us_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var conn = await _db.Collections.SingleOrDefaultAsync(x => x.Name == oldName && x.CreateUserId == us_id);
            if (conn is null)
            {
                return Ok(new { result = false, mesg = "操作的收藏夹不存在" });
            }
            conn.SetName(newName);
            _db.Collections.Update(conn);
            await _db.SaveChangesAsync();
            return Ok(new { result = true });
        }
        [HttpPost]
        public async Task<IActionResult> AddVideo([FromBody] AddConnectionRequest request)
        {
            if (!validator.Validate(request).IsValid) return BadRequest();
            long us_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var video = await _db.Video.SingleOrDefaultAsync(x => x.Id == request.videoId);
            if (video is null)
            {
                return Ok(new { result = false, mesg = "视频找不到了" });
            }

            var coll = await _db.Collections
            .Include(x => x.Item)
            .Where(x => x.CreateUserId == us_id)
            .Where(x => x.Item.Any(item => item.VideoId == request.videoId))
            .SingleOrDefaultAsync();
            var item = new CollectionItem(request.videoId);
            try
            {
                if (coll is not null)
                {
                    return Ok(new { result = false, mesg = "视频已收藏" });
                }
                var con = await _db.Collections.Include(x => x.Item).Where(x => x.CreateUserId == us_id && x.Name == request.name).SingleOrDefaultAsync();
                if (con is not null)
                {
                    _db.Collections.Update(con.AddItem(item));
                }
                else
                {
                    return Ok(new { result = false, mesg = "文件夹未存在" });
                }
                _db.Video.Update(video.AddCollect());
                await _db.SaveChangesAsync();
                return Ok(new { result = true, mesg = "收藏成功" });
            }
            catch (Exception e)
            {

                return Ok(new { result = false, mesg = "失败了~" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            long us_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cons = await _db.Collections.Where(x => x.CreateUserId == us_id).ToListAsync();
            return Ok(new { data = cons });
        }

        [AllowAnonymous]
        [HttpGet("{uid}")]
        public async Task<IActionResult> AccountList(long uid)
        {
            var claim = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            IQueryable<Collection> query = _db.Collections.Include(x => x.Item).ThenInclude(x => x.Video);
            if (claim is not null)
            {
                long us_id = long.Parse(claim);
                if (uid == us_id)
                {
                    query = query.Where(x => x.CreateUserId == uid);
                }
            }
            else
            {
                query = query.Where(x => x.CreateUserId == uid && x.IsVisible);
            }
            var con = await query.ToListAsync();
            return Ok(new { result = true, data = con });
        }
        [HttpPut("{name}")]
        public async Task<IActionResult> ChangeVisible(string name)
        {
            long u_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var conn =await _db.Collections.SingleOrDefaultAsync(x => x.Name == name && x.CreateUserId == u_id);
            if(conn is null)
            {
                return Ok(new { result = false,mesg = "没查询到呢"});
            }
            conn.SetVisible(!conn.IsVisible);
            _db.Collections.Update(conn);
            await _db.SaveChangesAsync();
            return Ok(new {result = true});
        }

       

    }
}
