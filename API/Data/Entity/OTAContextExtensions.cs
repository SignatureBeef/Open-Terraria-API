using System;
using OTA.Data;
using System.Linq;
using System.Collections.Generic;
using OTA.Data.Entity.Models;

namespace OTA.Data.Entity
{
    public static class OTAContextExtensions
    {
        public static IQueryable<DbPlayer> GetUser(this OTAContext ctx, string name)
        {
            return ctx.Players.Where(x => x.Name == name);
        }

        public static IQueryable<Group> GetUserGroups(this OTAContext ctx, int userId)
        {
            return ctx.Players
                .Where(x => x.Id == userId)
                .Join(ctx.PlayerGroups, pl => pl.Id, pn => pn.UserId, (pl, pg) => pg)
                .Join(ctx.Groups, pn => pn.GroupId, gr => gr.Id, (a, b) => b);
        }

        public static IQueryable<NodePermission> GetPermissionByNodeForUser(this OTAContext ctx, int userId, string node)
        {
            return ctx.Players
                .Where(x => x.Id == userId)
                .Join(ctx.PlayerNodes, pl => pl.Id, pn => pn.UserId, (pl, pn) => pn)
                .Join(ctx.Nodes, pn => pn.NodeId, np => np.Id, (a, b) => b)
                .Where(n => n.Node == node);
        }

        public static IQueryable<NodePermission> GetPermissionByNodeForGroup(this OTAContext ctx, int groupId, string node)
        {
            return ctx.Groups
                .Where(x => x.Id == groupId)
                .Join(ctx.GroupNodes, gr => gr.Id, pn => pn.GroupId, (pl, gn) => gn)
                .Join(ctx.Nodes, pn => pn.NodeId, np => np.Id, (a, b) => b)
                .Where(n => n.Node == node);
        }

        public static IQueryable<Group> GetParentForGroup(this OTAContext ctx, int groupId)
        {
            return ctx.Groups
                .Where(x => x.Id == groupId)
                .Join(ctx.Groups, g => g.Parent, sg => sg.Name, (a, b) => b);
//            return ctx.Groups
//                .Where(x => x.Id == groupId)
//                .Select(x => ctx.Groups.Where(y => y.Name == x.Parent).FirstOrDefault());
        }

        public static bool GuestGroupHasNode(this OTAContext ctx, string node, Permission permission)
        {
            return ctx.Groups
                .Where(x => x.ApplyToGuests)
                .Join(ctx.GroupNodes, gr => gr.Id, pn => pn.GroupId, (pl, gn) => gn)
                .Join(ctx.Nodes, pn => pn.NodeId, np => np.Id, (a, b) => b)
                .Any(n => n.Node == node && n.Permission == permission);
        }
    }
}

