using AutoMapper;
using TaskFlow.Api.Models;
using TaskFlow.Api.Models.Dtos;

namespace TaskFlow.Api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Map Project entity to ProjectDto
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.User,
                opt => opt.MapFrom(
                    src=> $"{src.Owner.Name} {src.Owner.LastName}"));
        // Map ProjectDto back to Project
        CreateMap<ProjectDto, Project>()
            .ForMember(dest => dest.Owner, opt => opt.Ignore()); // Ignore User field when mapping back

        // Map Comment entity to CommentDto
        CreateMap<Comment, CommentDto>().ReverseMap();;

        // Map Notification entity to NotificationDto
        CreateMap<Notification, NotificationDto>().ReverseMap();

        // Map ProjectTask entity to ProjectTaskDto
        CreateMap<ProjectTask, ProjectTaskDto>().ReverseMap();
    }
}