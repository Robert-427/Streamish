using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT Id, Name, Email, ImageUrl, DateCreated
                            FROM UserProfile
                            ORDER By DateCreated";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var profiles = new List<UserProfile>();
                        while (reader.Read())
                        {
                            profiles.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                            });
                        }
                        return profiles;
                    }
                }
            }
        }
        public UserProfile GetById(int id)
        {
            using (var comm = Connection)
            {
                comm.Open();
                using (var cmd = comm.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT Id, Name, Email, ImageUrl, DateCreated
                            FROM UserProfile
                            WHERE Id = @id";

                    DbUtils.AddParameter(cmd, "@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        UserProfile profile = null;
                        if (reader.Read())
                        {
                            profile = new UserProfile()
                            {
                                Id = id,
                                Name = DbUtils.GetString(reader, "Name"),
                                Email = DbUtils.GetString(reader, "Email"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                            };
                        }
                        return profile;
                    }
                }
            }
        }
        public UserProfile GetUserByIdWithVideos(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT	u.Id, u.Name, u.Email, u.ImageUrl, u.DateCreated AS UserDateCreated,
		                            v.Id AS VideoId, v.Title, v.Description, v.Url, v.DateCreated AS VideoDateCreated, v.UserProfileId,
                                    c.Id AS CommentId, c.Message, c.VideoId AS CommentVideoId, c.UserProfileId AS CommentUserId
                            FROM UserProfile u
                            JOIN Video v ON v.UserProfileId = u.Id
                            LEFT JOIN Comment c ON c.VideoId = v.Id
                            WHERE u.Id = @Id";

                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        UserProfile user = null;
                        while (reader.Read())
                        {
                            if (user == null)
                            {
                                user = new UserProfile()
                                {
                                    Id = id,
                                    Name=DbUtils.GetString(reader, "Name"),
                                    Email=DbUtils.GetString(reader, "Email"),
                                    ImageUrl=DbUtils.GetString(reader, "ImageUrl"),
                                    DateCreated=DbUtils.GetDateTime(reader, "UserDateCreated"),
                                    Video = new List<Video>()
                                };
                            }
                            List<Comment> comments = new List<Comment>();
                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                                user.Video.Add(new Video()
                                {
                                    Id = DbUtils.GetInt(reader, "VideoId"),
                                    Title = DbUtils.GetString(reader, "Title"),
                                    Description = DbUtils.GetString(reader, "Description"),
                                    Url = DbUtils.GetString(reader, "Url"),
                                    DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                    UserProfileId = id,
                                    Comments = comments
                                });
                                {
                                    if (DbUtils.IsNotDbNull(reader, "CommentId"))
                                    {
                                        comments.Add(new Comment()
                                        {
                                            Id = DbUtils.GetInt(reader, "CommentId"),
                                            Message = DbUtils.GetString(reader, "Message"),
                                            VideoId = DbUtils.GetInt(reader, "VideoId"),
                                            UserProfileId = id,
                                        });
                                    }
                                }
                            }
                        }
                        return user;
                    }
                }
            }
        }
        public void Add(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            INSERT INTO UserProfile (Name, Email, ImageUrl, DateCreated)
                            OUTPUT INSERTED.ID
                            VALUES (@Name, @Email, @ImageUrl, @DateCreated)";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
        public void Update(UserProfile user)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE UserProfile
                            SET Name = @Name,
                                Email = @Email,
                                ImageUrl = @ImageUrl,
                                DateCreated = @DateCreated
                            WHERE Id = @id";

                    DbUtils.AddParameter(cmd, "@Name", user.Name);
                    DbUtils.AddParameter(cmd, "@Email", user.Email);
                    DbUtils.AddParameter(cmd, "@ImageUrl", user.ImageUrl);
                    DbUtils.AddParameter(cmd, "@DateCreated", user.DateCreated);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
