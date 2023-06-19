using System.Collections.Generic;
using RWCustom;
using UnityEngine;

namespace TestMod
{
    public class Crate : PhysicalObject, IDrawable
    {
        public Crate(CrateAbstract abstr) : base(abstr)
        {
            float mass = 40f;
            var positions = new List<Vector2>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    positions.Add(new Vector2(x, y) * 20);
                }
            }

            

            bodyChunks = new BodyChunk[positions.Count];

            // Create all body chunks
            for(int i = 0; i < bodyChunks.Length; i++)
            {
                bodyChunks[i] = new BodyChunk(this, i, Vector2.zero, 30f, mass / bodyChunks.Length);
            }

            // Scale up the middle chunk
            bodyChunks[4].rad = 40f;

           
            bodyChunkConnections = new BodyChunkConnection[bodyChunks.Length * (bodyChunks.Length - 1) / 2];
            int connection = 0;

            // Create all chunk connections
            
            for (int x = 0; x < bodyChunks.Length; x++)
            {
                for (int y = x + 1; y < bodyChunks.Length; y++)
                {
                    bodyChunkConnections[connection] = new BodyChunkConnection(bodyChunks[x], bodyChunks[y], Vector2.Distance(positions[x], positions[y]), BodyChunkConnection.Type.Normal, 0.5f, -1f);
                    connection++;
                }
            }
            

            airFriction = 0.999f;
            gravity = 0.9f;
            bounce = 0.3f;
            surfaceFriction = 0.5f;
            collisionLayer = 1;
            waterFriction = 0.92f;
            buoyancy = 0.75f;
            GoThroughFloors = false;
        }

        public override void Update(bool eu)
        {
            base.Update(eu);

            if (grabbedBy.Count == 0)
            {
                // Slows crate down to stop the "slipperyness" that it has when slippin' accross the floor
                bodyChunks[0].vel = new Vector2(bodyChunks[0].vel.x *= 0.65f, bodyChunks[0].vel.y);
            }
        }

        public override void PlaceInRoom(Room placeRoom)
        {
            base.PlaceInRoom(placeRoom);

            Vector2 center = placeRoom.MiddleOfTile(abstractPhysicalObject.pos);

            int i = 0;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    bodyChunks[i].HardSetPosition(new Vector2(x, y) * 20f + center);
                    i++;
                }
            }
        }

        public override void Collide(PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            base.Collide(otherObject, myChunk, otherChunk);
            if (otherObject.bodyChunks[otherChunk].owner is Creature creature && !creature.dead)
            {
                float damage = (this.bodyChunks[myChunk].vel.x + this.bodyChunks[myChunk].vel.y) / 2;
                if (damage < 0)
                {
                    damage *= -1;
                }
                else if (damage < 1)
                {
                    // Don't deal damage
                }
                else
                {
                    creature.Violence(this.bodyChunks[myChunk], this.bodyChunks[myChunk].vel, otherObject.bodyChunks[otherChunk], null, Creature.DamageType.Blunt, damage, 5f);
                    //Debug.Log("Crate collision damage: " + damage);
                }
            }

        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);

            if (speed > 10)
            {
                room.PlaySound(SoundID.Spear_Fragment_Bounce, bodyChunks[chunk].pos, 0.35f, 2f);
            }
        }

        public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            sLeaser.sprites = new FSprite[bodyChunks.Length];
            
            for(int i = 0; i < bodyChunks.Length; i++)
                sLeaser.sprites[i] = new FSprite("Circle20");

            AddToContainer(sLeaser, rCam, null);
        }

        public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            for(int i = 0; i < bodyChunks.Length; i++)
            {
                var spr = sLeaser.sprites[i];
                spr.SetPosition(Vector2.Lerp(bodyChunks[i].lastPos, bodyChunks[i].pos, timeStacker) - camPos);
                spr.scale = bodyChunks[i].rad / 10f;
            }

            if (slatedForDeletetion || room != rCam.room)
                sLeaser.CleanSpritesAndRemove();
        }

        public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
        {
            foreach (var sprite in sLeaser.sprites)
                sprite.color = palette.blackColor;
        }

        public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
        {
            newContainer ??= rCam.ReturnFContainer("Items");

            foreach (FSprite fsprite in sLeaser.sprites)
                newContainer.AddChild(fsprite);
        }
    }
}
