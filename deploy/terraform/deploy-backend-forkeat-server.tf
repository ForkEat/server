resource "digitalocean_database_cluster" "postgres" {
  name       = "postgres-prod"
  engine     = "pg"
  version    = "13"
  size       = "db-s-1vcpu-1gb"
  node_count = 1
  region = "${var.region}"
}

resource "digitalocean_droplet" "web" {
  name   = "forkeat-backend"
  size   = "s-1vcpu-1gb"
  image  = "debian-11-x64"
  region = "${var.region}"

  ssh_keys = [
      data.digitalocean_ssh_key.terraform.id
  ]


  provisioner "remote-exec" {
    inline = ["sudo apt update", "sudo apt install python3 -y", "echo Done!"]

    connection {
      host        = self.ipv4_address
      type        = "ssh"
      user        = "root"
      private_key = file(var.pvt_key)
    }
  }

  provisioner "local-exec" {
    command = "ANSIBLE_HOST_KEY_CHECKING=False ansible-playbook -u root -i '${self.ipv4_address},' --private-key ${var.pvt_key} -e 'pub_key=${var.pub_key}' --extra-vars='{\"DATABASE_URL\": ${digitalocean_database_cluster.postgres.private_uri} ../ansible/run.yml"
  }
}

# resource "digitalocean_loadbalancer" "public" {
#   name   = "loadbalancer-1"

#   forwarding_rule {
#     entry_port     = 80
#     entry_protocol = "http"

#     target_port     = 80
#     target_protocol = "http"
#   }

#   healthcheck {
#     port     = 80
#     protocol = "http"
#   }

#   droplet_ids = [digitalocean_droplet.web.id]
# }