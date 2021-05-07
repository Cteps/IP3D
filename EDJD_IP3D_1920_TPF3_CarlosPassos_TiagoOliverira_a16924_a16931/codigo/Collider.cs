using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto_3D
{
    public interface Collider
    {
        // Obter nome do objeto que tem o collider
        string Name();
        // Notificar objeto que houve colisao
        void CollisionWith(Collider other);
        // Validar colisões (existe/não existe)
        bool CollidesWith(Collider other);
        // Retorna o collider do objeto (Circle)
        Collider GetCollider();
    }
}
