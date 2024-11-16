using AutoMapper;
using TaskFlow.Api.Models;
using TaskFlow.Api.Models.Dtos;

namespace TaskFlow.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map Project entity to ProjectDto
        CreateMap<Project, ProjectDto>().ReverseMap();

        // Map Comment entity to CommentDto
        CreateMap<Comment, CommentDto>().ReverseMap();;

        // Map Notification entity to NotificationDto
        CreateMap<Notification, NotificationDto>().ReverseMap();

        // Map ProjectTask entity to ProjectTaskDto
        CreateMap<ProjectTask, ProjectTaskDto>().ReverseMap();
    }
}